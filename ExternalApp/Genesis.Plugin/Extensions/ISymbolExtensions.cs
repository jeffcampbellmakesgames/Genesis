using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace Genesis.Plugin
{
	/// <summary>
	///     Helper methods for <see cref="ISymbol" />.
	/// </summary>
	public static class ISymbolExtensions
	{
		#region Symbol Visitors

		private class IsUnsafeVisitor : SymbolVisitor<bool>
		{
			private readonly HashSet<ISymbol> _visited = new HashSet<ISymbol>();

			public override bool DefaultVisit(ISymbol symbol)
			{
				return false;
			}

			public override bool VisitArrayType(IArrayTypeSymbol symbol)
			{
				if (!_visited.Add(symbol)) return false;

				return symbol.ElementType.Accept(this);
			}

			public override bool VisitDynamicType(IDynamicTypeSymbol symbol)
			{
				// The dynamic type is never unsafe (well....you know what I mean
				return false;
			}

			public override bool VisitField(IFieldSymbol symbol)
			{
				if (!_visited.Add(symbol)) return false;

				return symbol.Type.Accept(this);
			}

			public override bool VisitNamedType(INamedTypeSymbol symbol)
			{
				if (!_visited.Add(symbol)) return false;

				return symbol.GetAllTypeArguments().Any(ts => ts.Accept(this));
			}

			public override bool VisitPointerType(IPointerTypeSymbol symbol)
			{
				if (!_visited.Add(symbol)) return false;

				return true;
			}

			public override bool VisitProperty(IPropertySymbol symbol)
			{
				if (!_visited.Add(symbol)) return false;

				return
					symbol.Type.Accept(this) ||
					symbol.Parameters.Any(p => p.Accept(this));
			}

			public override bool VisitTypeParameter(ITypeParameterSymbol symbol)
			{
				if (!_visited.Add(symbol)) return false;

				return symbol.ConstraintTypes.Any(ts => ts.Accept(this));
			}

			public override bool VisitMethod(IMethodSymbol symbol)
			{
				if (!_visited.Add(symbol)) return false;

				return
					symbol.ReturnType.Accept(this) ||
					symbol.Parameters.Any(p => p.Accept(this)) ||
					symbol.TypeParameters.Any(tp => tp.Accept(this));
			}

			public override bool VisitParameter(IParameterSymbol symbol)
			{
				if (!_visited.Add(symbol)) return false;

				return symbol.Type.Accept(this);
			}

			public override bool VisitEvent(IEventSymbol symbol)
			{
				if (!_visited.Add(symbol)) return false;

				return symbol.Type.Accept(this);
			}

			public override bool VisitAlias(IAliasSymbol symbol)
			{
				if (!_visited.Add(symbol)) return false;

				return symbol.Target.Accept(this);
			}
		}

		#endregion

		/// <summary>
		/// Returns all <see cref="ITypeSymbol"/> arguments for this <see cref="ISymbol"/>.
		/// </summary>
		public static ImmutableArray<ITypeSymbol> GetTypeArguments(this ISymbol symbol)
		{
			return symbol switch
			{
				IMethodSymbol m => m.TypeArguments,
				INamedTypeSymbol nt => nt.TypeArguments,
				_ => ImmutableArray.Create<ITypeSymbol>()
			};
		}

		/// <summary>
		/// Returns all declaring <see cref="SyntaxReference"/>s of this <see cref="ISymbol"/>. None are returned
		/// if this <see cref="ISymbol"/> is implicitly declared or declared in metadata.
		/// </summary>
		public static IEnumerable<SyntaxReference> GetDeclarations(this ISymbol symbol)
		{
			return symbol != null
				? symbol.DeclaringSyntaxReferences.AsEnumerable()
				: EmptyTools.EmptyEnumerable<SyntaxReference>();
		}

		/// <summary>
		/// Returns all <see cref="DeclarationModifiers"/> for this <see cref="ISymbol"/>.
		/// </summary>
		public static DeclarationModifiers GetSymbolModifiers(this ISymbol symbol)
		{
			// ported from roslyn source - why they didn't use DeclarationModifiers.From (symbol) ?
			return DeclarationModifiers.None
				.WithIsStatic(symbol.IsStatic)
				.WithIsAbstract(symbol.IsAbstract)
				.WithIsUnsafe(symbol.IsUnsafe())
				.WithIsVirtual(symbol.IsVirtual)
				.WithIsOverride(symbol.IsOverride)
				.WithIsSealed(symbol.IsSealed);
		}

		/// <summary>
		/// Returns the contained member of this <see cref="ISymbol"/> or this if it is the contained member.
		/// </summary>
		public static ISymbol GetContainingMemberOrThis(this ISymbol symbol)
		{
			if (symbol == null)
			{
				return null;
			}

			switch (symbol.Kind)
			{
				case SymbolKind.Assembly:
				case SymbolKind.NetModule:
				case SymbolKind.Namespace:
				case SymbolKind.Preprocessing:
				case SymbolKind.Alias:
				case SymbolKind.ArrayType:
				case SymbolKind.DynamicType:
				case SymbolKind.ErrorType:
				case SymbolKind.NamedType:
				case SymbolKind.PointerType:
				case SymbolKind.Label:
					throw new NotSupportedException();
				case SymbolKind.Field:
				case SymbolKind.Property:
				case SymbolKind.Event:
					return symbol;
				case SymbolKind.Method:
					if (symbol.IsAccessorMethod())
						return ((IMethodSymbol) symbol).AssociatedSymbol;
					return symbol;
				case SymbolKind.Local:
				case SymbolKind.Parameter:
				case SymbolKind.TypeParameter:
				case SymbolKind.RangeVariable:
					return GetContainingMemberOrThis(symbol.ContainingSymbol);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Returns all <see cref="IParameterSymbol"/>s for this <see cref="ISymbol"/>.
		/// </summary>
		public static ImmutableArray<IParameterSymbol> GetParameters(this ISymbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			if (symbol is IMethodSymbol method)
			{
				return method.Parameters;
			}

			if (symbol is IPropertySymbol property)
			{
				return property.Parameters;
			}

			if (symbol is IEventSymbol ev)
			{
				return ev.Type.GetDelegateInvokeMethod().Parameters;
			}

			return ImmutableArray<IParameterSymbol>.Empty;
		}

		/// <summary>
		/// Returns all <see cref="ITypeParameterSymbol"/>s for this <see cref="ISymbol"/>.
		/// </summary>
		public static ImmutableArray<ITypeParameterSymbol> GetTypeParameters(this ISymbol symbol)
		{
			return symbol switch
			{
				null => throw new ArgumentNullException(nameof(symbol)),
				INamedTypeSymbol type => type.TypeParameters,
				IMethodSymbol method => method.TypeParameters,
				_ => ImmutableArray<ITypeParameterSymbol>.Empty
			};
		}

		/// <summary>
		///     Returns true if this <see cref="ISymbol"/> has <see cref="Attribute" />-derived type
		///     <typeparamref name="T" />.
		/// </summary>
		/// <exception cref="Exception">
		///     <typeparamref name="T" /> must be an attribute, otherwise an assertion
		///     will be thrown.
		/// </exception>
		public static bool HasAttribute<T>(this ISymbol symbol)
		{
			if (!typeof(Attribute).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException("T must be assignable to Attribute.");
			}

			return symbol.GetAttributes().Any(attr =>
				attr.AttributeClass != null &&
				attr.AttributeClass.Name == typeof(T).Name);
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is an instance or static constructor, otherwise false.
		/// </summary>
		public static bool IsAnyConstructor(this ISymbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			return symbol is IMethodSymbol method &&
			       (method.MethodKind == MethodKind.Constructor || method.MethodKind == MethodKind.StaticConstructor);
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is an instance constructor, otherwise false.
		/// </summary>
		public static bool IsConstructor(this ISymbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			return symbol is IMethodSymbol methodSymbol &&
			       methodSymbol.MethodKind == MethodKind.Constructor;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a static constructor, otherwise false.
		/// </summary>
		public static bool IsStaticConstructor(this ISymbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			return symbol is IMethodSymbol methodSymbol &&
			       methodSymbol.MethodKind == MethodKind.StaticConstructor;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a destructor, otherwise false.
		/// </summary>
		public static bool IsDestructor(this ISymbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			return symbol is IMethodSymbol methodSymbol &&
			       methodSymbol.MethodKind == MethodKind.Destructor;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a delegate type, otherwise false.
		/// </summary>
		public static bool IsDelegateType(this ISymbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			return symbol is ITypeSymbol typeSymbol &&
			       typeSymbol.TypeKind == TypeKind.Delegate;
		}

		/// <summary>
		/// Returns the return type of this <see cref="ISymbol"/>, if any. Otherwise returns null.
		/// </summary>
		public static ITypeSymbol GetReturnType(this ISymbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			switch (symbol.Kind)
			{
				case SymbolKind.Field:
					var field = (IFieldSymbol) symbol;
					return field.Type;
				case SymbolKind.Method:
					var method = (IMethodSymbol) symbol;
					if (method.MethodKind == MethodKind.Constructor)
						return method.ContainingType;
					return method.ReturnType;
				case SymbolKind.Property:
					var property = (IPropertySymbol) symbol;
					return property.Type;
				case SymbolKind.Event:
					var evt = (IEventSymbol) symbol;
					return evt.Type;
				case SymbolKind.Parameter:
					var param = (IParameterSymbol) symbol;
					return param.Type;
				case SymbolKind.Local:
					var local = (ILocalSymbol) symbol;
					return local.Type;
			}

			return null;
		}

		/// <summary>
		///     Returns true if this <see cref="ISymbol" /> is a type, otherwise false.
		/// </summary>
		public static bool IsType(this ISymbol symbol)
		{
			return symbol is ITypeSymbol typeSymbol && typeSymbol.IsType;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is an accessor for a property or event, otherwise false.
		/// </summary>
		public static bool IsAccessorMethod(this ISymbol symbol)
		{
			return symbol is IMethodSymbol accessorSymbol &&
			       (accessorSymbol.MethodKind == MethodKind.PropertySet ||
			        accessorSymbol.MethodKind == MethodKind.PropertyGet ||
			        accessorSymbol.MethodKind == MethodKind.EventRemove ||
			        accessorSymbol.MethodKind == MethodKind.EventAdd);
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is public, otherwise false.
		/// </summary>
		public static bool IsPublic(this ISymbol symbol)
		{
			return symbol.DeclaredAccessibility == Accessibility.Public;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is protected, otherwise false.
		/// </summary>
		public static bool IsProtected(this ISymbol symbol)
		{
			return symbol.DeclaredAccessibility == Accessibility.Protected;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is an error type, otherwise false.
		/// </summary>
		public static bool IsErrorType(this ISymbol symbol)
		{
			return symbol is ITypeSymbol typeSymbol &&
			       typeSymbol.TypeKind == TypeKind.Error;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is an indexer, otherwise false.
		/// </summary>
		public static bool IsIndexer(this ISymbol symbol)
		{
			return (symbol as IPropertySymbol)?.IsIndexer == true;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a user-defined operator, otherwise false.
		/// </summary>
		public static bool IsUserDefinedOperator(this ISymbol symbol)
		{
			return (symbol as IMethodSymbol)?.MethodKind == MethodKind.UserDefinedOperator;
		}

		/// <summary>
		/// Returns the visibility of this <see cref="ISymbol"/>.
		/// </summary>
		public static SymbolVisibility GetResultantVisibility(this ISymbol symbol)
		{
			// Start by assuming it's visible.
			var visibility = SymbolVisibility.Public;
			switch (symbol.Kind)
			{
				case SymbolKind.Alias:
					// Aliases are uber private.  They're only visible in the same file that they
					// were declared in.
					return SymbolVisibility.Private;

				case SymbolKind.Parameter:
					// Parameters are only as visible as their containing symbol
					return GetResultantVisibility(symbol.ContainingSymbol);

				case SymbolKind.TypeParameter:
					// Type Parameters are private.
					return SymbolVisibility.Private;
			}

			while (symbol != null && symbol.Kind != SymbolKind.Namespace)
			{
				switch (symbol.DeclaredAccessibility)
				{
					// If we see anything private, then the symbol is private.
					case Accessibility.NotApplicable:
					case Accessibility.Private:
						return SymbolVisibility.Private;

					// If we see anything internal, then knock it down from public to
					// internal.
					case Accessibility.Internal:
					case Accessibility.ProtectedAndInternal:
						visibility = SymbolVisibility.Internal;
						break;

					// For anything else (Public, Protected, ProtectedOrInternal), the
					// symbol stays at the level we've gotten so far.
				}

				symbol = symbol.ContainingSymbol;
			}

			return visibility;
		}

		/// <summary>
		///     Returns true if <paramref name="symbol" /> is an Anonymous type, otherwise false.
		/// </summary>
		public static bool IsAnonymousType(this ISymbol symbol)
		{
			return symbol is INamedTypeSymbol namedTypeSymbol &&
			       namedTypeSymbol.IsAnonymousType;
		}

		/// <summary>
		/// Return true if this <see cref="ISymbol"/> is a <see cref="IPointerTypeSymbol"/>, otherwise false.
		/// </summary>
		public static bool IsPointerType(this ISymbol symbol)
		{
			return symbol is IPointerTypeSymbol;
		}

		/// <summary>
		///     Returns true if <paramref name="symbol" /> is an Module type, otherwise false.
		/// </summary>
		public static bool IsModuleType(this ISymbol symbol)
		{
			return (symbol as ITypeSymbol)?.IsModuleType() == true;
		}

		/// <summary>
		///     Returns true if <paramref name="symbol" /> is an Interface type, otherwise false.
		/// </summary>
		public static bool IsInterfaceType(this ISymbol symbol)
		{
			return (symbol as ITypeSymbol)?.IsInterfaceType() == true;
		}

		/// <summary>
		///     Returns true if <paramref name="symbol" /> is an array, otherwise false.
		/// </summary>
		public static bool IsArrayType(this ISymbol symbol)
		{
			return symbol?.Kind == SymbolKind.ArrayType;
		}

		/// <summary>
		///     Returns true if <paramref name="symbol" /> is an Anonymous return type method, otherwise false.
		/// </summary>
		public static bool IsAnonymousFunction(this ISymbol symbol)
		{
			return (symbol as IMethodSymbol)?.MethodKind == MethodKind.AnonymousFunction;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol.Kind"/> equals <paramref name="kind"/>.
		/// </summary>
		public static bool IsKind(this ISymbol symbol, SymbolKind kind)
		{
			return symbol.MatchesKind(kind);
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol.Kind"/> equals <paramref name="kind"/>.
		/// </summary>
		public static bool MatchesKind(this ISymbol symbol, SymbolKind kind)
		{
			return symbol?.Kind == kind;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a reduced extension method, otherwise false (i.e, an
		/// extension method without the this parameter).
		/// </summary>
		public static bool IsReducedExtension(this ISymbol symbol)
		{
			return symbol is IMethodSymbol methodSymbol &&
			       methodSymbol.MethodKind == MethodKind.ReducedExtension;
		}

		/// <summary>
		/// Returns true this <see cref="ISymbol"/> is an extension method, otherwise false.
		/// </summary>
		public static bool IsExtensionMethod(this ISymbol symbol)
		{
			return symbol.Kind == SymbolKind.Method && ((IMethodSymbol) symbol).IsExtensionMethod;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is an ordinary method, otherwise false.
		/// </summary>
		public static bool IsConversion(this ISymbol symbol)
		{
			return (symbol as IMethodSymbol)?.MethodKind == MethodKind.Conversion;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is an ordinary method, otherwise false.
		/// </summary>
		public static bool IsOrdinaryMethod(this ISymbol symbol)
		{
			return (symbol as IMethodSymbol)?.MethodKind == MethodKind.Ordinary;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a normal anonymous type.
		/// </summary>
		public static bool IsNormalAnonymousType(this ISymbol symbol)
		{
			return symbol.IsAnonymousType() && !symbol.IsDelegateType();
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a nullable type or returns a nullable value, otherwise false.
		/// </summary>
		public static bool IsNullableType(this ISymbol symbol)
		{
			switch (symbol)
			{
				case IFieldSymbol fieldSymbol:
					return fieldSymbol.NullableAnnotation == NullableAnnotation.Annotated;
				case IPropertySymbol propertySymbol:
					return propertySymbol.NullableAnnotation == NullableAnnotation.Annotated;
				case IMethodSymbol methodSymbol:
					return methodSymbol.ReturnType.IsNullable();
			}

			return false;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is an anonymous delegate type, otherwise false.
		/// </summary>
		public static bool IsAnonymousDelegateType(this ISymbol symbol)
		{
			return symbol.IsAnonymousType() && symbol.IsDelegateType();
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a write-able field or property, otherwise false.
		/// </summary>
		public static bool IsWriteableFieldOrProperty(this ISymbol symbol)
		{
			return symbol switch
			{
				IFieldSymbol fieldSymbol => !fieldSymbol.IsReadOnly && !fieldSymbol.IsConst,
				IPropertySymbol propertySymbol => !propertySymbol.IsReadOnly,
				_ => false
			};
		}

		/// <summary>
		/// Returns the underlying type of the nullable, if present.
		/// </summary>
		public static ITypeSymbol GetNullableUnderlyingType(this ISymbol symbol)
		{
			if (!IsNullableType(symbol))
			{
				return null;
			}

			return symbol switch
			{
				IFieldSymbol fieldSymbol => fieldSymbol.Type.GetTypeArguments()[0],
				IPropertySymbol propertySymbol => propertySymbol.Type.GetTypeArguments()[0],
				_ => null
			};
		}

		/// <summary>
		/// Returns the <see cref="ITypeSymbol"/> of this <paramref name="symbol"/> instance.
		/// </summary>
		public static ITypeSymbol GetMemberType(this ISymbol symbol)
		{
			return symbol.Kind switch
			{
				SymbolKind.Field => ((IFieldSymbol) symbol).Type,
				SymbolKind.Property => ((IPropertySymbol) symbol).Type,
				SymbolKind.Method => ((IMethodSymbol) symbol).ReturnType,
				SymbolKind.Event => ((IEventSymbol) symbol).Type,
				SymbolKind.Alias => ((IAliasSymbol) symbol).Target as ITypeSymbol,
				SymbolKind.Local => ((ILocalSymbol) symbol).Type,
				_ => null
			};
		}

		/// <summary>
		/// Returns the Arity of this <see cref="ISymbol"/> (i.e, the number of type parameters this
		/// <see cref="ISymbol"/> accepts.
		/// </summary>
		public static int GetArity(this ISymbol symbol)
		{
			return symbol.Kind switch
			{
				SymbolKind.NamedType => ((INamedTypeSymbol) symbol).Arity,
				SymbolKind.Method => ((IMethodSymbol) symbol).Arity,
				_ => 0
			};
		}

		/// <summary>
		///     Returns the fully-qualified type name of this <paramref name="symbol" />.
		/// </summary>
		public static string GetFullTypeName(this ISymbol symbol)
		{
			return symbol.GetMemberType().ToString();
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a "this" parameter, otherwise false.
		/// </summary>
		public static bool IsThisParameter(this ISymbol symbol)
		{
			return symbol != null && symbol.Kind == SymbolKind.Parameter && ((IParameterSymbol) symbol).IsThis;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a "params" parameter, otherwise false.
		/// </summary>
		public static bool IsParams(this ISymbol symbol)
		{
			var parameters = symbol.GetParameters();
			return parameters.Length > 0 && parameters[parameters.Length - 1].IsParams;
		}

		/// <summary>
		/// Returns all type argument <see cref="ITypeSymbol"/>s for this <see cref="ISymbol"/>.
		/// </summary>
		public static ImmutableArray<ITypeSymbol> GetAllTypeArguments(this ISymbol symbol)
		{
			var results = new List<ITypeSymbol>(symbol.GetTypeArguments());

			var containingType = symbol.ContainingType;
			while (containingType != null)
			{
				results.AddRange(containingType.GetTypeArguments());
				containingType = containingType.ContainingType;
			}

			return ImmutableArray.CreateRange(results);
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is an attribute, otherwise false.
		/// </summary>
		public static bool IsAttribute(this ISymbol symbol)
		{
			return (symbol as ITypeSymbol)?.IsAttribute() == true;
		}

		/// <summary>
		///     Returns true if this symbol contains anything unsafe within it.  for example
		///     List&lt;int*[]&gt; is unsafe, as it "int* Foo { get; }"
		/// </summary>
		public static bool IsUnsafe(this ISymbol member)
		{
			return member?.Accept(new IsUnsafeVisitor()) == true;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is deprecated, otherwise false.
		/// </summary>
		public static bool IsDeprecated(this ISymbol symbol)
		{
			return symbol.HasAttribute<ObsoleteAttribute>();
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a static type, otherwise false.
		/// </summary>
		public static bool IsStaticType(this ISymbol symbol)
		{
			return symbol != null && symbol.Kind == SymbolKind.NamedType && symbol.IsStatic;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is a namespace, otherwise false.
		/// </summary>
		public static bool IsNamespace(this ISymbol symbol)
		{
			return symbol?.Kind == SymbolKind.Namespace;
		}

		/// <summary>
		/// Returns true if this <see cref="ISymbol"/> is an event accessor, otherwise false.
		/// </summary>
		public static bool IsEventAccessor(this ISymbol symbol)
		{
			return symbol is IMethodSymbol method &&
			       (method.MethodKind == MethodKind.EventAdd ||
			        method.MethodKind == MethodKind.EventRaise ||
			        method.MethodKind == MethodKind.EventRemove);
		}

		/// <summary>
		/// Returns the <see cref="ITypeSymbol"/> for this <see cref="ISymbol"/>.
		/// </summary>
		public static ITypeSymbol GetSymbolType(this ISymbol symbol)
		{
			return symbol switch
			{
				ILocalSymbol localSymbol => localSymbol.Type,
				IFieldSymbol fieldSymbol => fieldSymbol.Type,
				IPropertySymbol propertySymbol => propertySymbol.Type,
				IParameterSymbol parameterSymbol => parameterSymbol.Type,
				IAliasSymbol aliasSymbol => aliasSymbol.Target as ITypeSymbol,
				_ => symbol as ITypeSymbol
			};
		}
	}
}
