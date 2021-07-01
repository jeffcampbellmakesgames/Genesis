/*

MIT License

Copyright (c) Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	///     Helper methods for <see cref="ITypeSymbol" />
	/// </summary>
	public static class ITypeSymbolExtensions
	{
		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> implements a special type interface of
		/// <paramref name="type"/>.
		/// </summary>
		public static bool ImplementsSpecialTypeInterface(this ITypeSymbol symbol, SpecialType type)
		{
			if (symbol.SpecialType == type)
			{
				return true;
			}

			switch (symbol)
			{
				case INamedTypeSymbol namedType when namedType.IsGenericType &&
				                                     !SymbolEqualityComparer.Default.Equals(namedType, namedType.ConstructedFrom):
					return namedType.ConstructedFrom.ImplementsSpecialTypeInterface(type);
				case ITypeParameterSymbol typeParam:
					return typeParam.ConstraintTypes.Any(x => x.ImplementsSpecialTypeInterface(type));
			}

			return symbol.AllInterfaces.Any(x => x.ImplementsSpecialTypeInterface(type));
		}

		/// <summary>
		///     Gets the invoke method for a delegate type.
		/// </summary>
		/// <remarks>
		///     Returns null if the type is not a delegate type; or if the invoke method could not be found.
		/// </remarks>
		public static IMethodSymbol GetDelegateInvokeMethod(this ITypeSymbol type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (type.TypeKind == TypeKind.Delegate)
				return type.GetMembers("Invoke").OfType<IMethodSymbol>()
					.FirstOrDefault(m => m.MethodKind == MethodKind.DelegateInvoke);
			return null;
		}

		/// <summary>
		/// Returns all <see cref="INamedTypeSymbol"/>s for all interfaces on this <paramref name="typeSymbol"/>
		/// including itself if it's also an interface.
		/// </summary>
		public static IList<INamedTypeSymbol> GetAllInterfacesIncludingThis(this ITypeSymbol typeSymbol)
		{
			var allInterfaces = typeSymbol.AllInterfaces;
			if (typeSymbol is INamedTypeSymbol namedType &&
			    namedType.TypeKind == TypeKind.Interface && !allInterfaces.Contains(namedType))
			{
				var result = new List<INamedTypeSymbol>(allInterfaces.Length + 1);
				result.Add(namedType);
				result.AddRange(allInterfaces);
				return result;
			}

			return allInterfaces;
		}

		/// <summary>
		///     Returns true if this <see cref="ITypeSymbol" /> implements an interface matching
		/// <paramref name="interfaceTypeName" />.
		/// </summary>
		public static bool ImplementsInterface(this ITypeSymbol typeSymbol, string interfaceTypeName)
		{
			return typeSymbol.Name != interfaceTypeName &&
			       typeSymbol.AllInterfaces.Any(interfaceTypeSymbol => interfaceTypeSymbol.Name == interfaceTypeName);
		}

		/// <summary>
		/// Returns true if this <paramref name="typeSymbol"/> is decorated with interface type
		/// <typeparamref name="T"/>.
		/// </summary>
		/// <exception cref="ArgumentException">
		///     <typeparamref name="T" /> must be an interface, otherwise an exception will be thrown.
		/// </exception>
		public static bool ImplementsInterface<T>(this ITypeSymbol typeSymbol)
		{
			if (!typeof(T).IsInterface)
			{
				throw new ArgumentException("T must be an Interface.");
			}

			var interfaceName = typeof(T).Name;

			return typeSymbol.ImplementsInterface(interfaceName);
		}

		/// <summary>
		///     Returns all accessible members of this <paramref name="typeSymbol" /> and all base types.
		/// </summary>
		public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol typeSymbol)
		{
			var allTypes = typeSymbol.GetBaseTypesAndThis();
			return allTypes.SelectMany(x =>
			{
				return x.GetMembers()
					.Where(y => y.DeclaredAccessibility == Accessibility.Public);
			});
		}

		/// <summary>
		///     Returns the name of the assembly that contains this <see cref="INamedTypeSymbol" />.
		/// </summary>
		public static string GetAssemblyName(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.ContainingAssembly.Identity.Name;
		}

		/// <summary>
		///     Returns true if <paramref name="typeSymbol" /> is an array, otherwise false.
		/// </summary>
		public static bool IsArrayType(this ITypeSymbol typeSymbol)
		{
			return typeSymbol?.TypeKind == TypeKind.Array;
		}

		/// <summary>
		///     Returns true if <paramref name="typeSymbol" /> is an abstract class, otherwise false.
		/// </summary>
		public static bool IsAbstractClass(this ITypeSymbol typeSymbol)
		{
			return typeSymbol?.TypeKind == TypeKind.Class && typeSymbol.IsAbstract;
		}

		/// <summary>
		///     Returns true if <paramref name="typeSymbol" /> is a Void type, otherwise false.
		/// </summary>
		public static bool IsSystemVoid(this ITypeSymbol typeSymbol)
		{
			return typeSymbol?.SpecialType == SpecialType.System_Void;
		}

		/// <summary>
		///     Returns true if <paramref name="typeSymbol" /> is a Nullable type, otherwise false.
		/// </summary>
		public static bool IsNullable(this ITypeSymbol typeSymbol)
		{
			return typeSymbol?.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
		}

		/// <summary>
		///     Returns true if <paramref name="typeSymbol" /> is an Error type, otherwise false.
		/// </summary>
		public static bool IsErrorType(this ITypeSymbol typeSymbol)
		{
			return typeSymbol?.TypeKind == TypeKind.Error;
		}

		/// <summary>
		///     Returns true if <paramref name="typeSymbol" /> is an Module type, otherwise false.
		/// </summary>
		public static bool IsModuleType(this ITypeSymbol typeSymbol)
		{
			return typeSymbol?.TypeKind == TypeKind.Module;
		}

		/// <summary>
		///     Returns true if <paramref name="typeSymbol" /> is an Interface type, otherwise false.
		/// </summary>
		public static bool IsInterfaceType(this ITypeSymbol typeSymbol)
		{
			return typeSymbol?.TypeKind == TypeKind.Interface;
		}

		/// <summary>
		///     Returns true if <paramref name="typeSymbol" /> is an Delegate type, otherwise false.
		/// </summary>
		public static bool IsDelegateType(this ITypeSymbol typeSymbol)
		{
			return typeSymbol?.TypeKind == TypeKind.Delegate;
		}

		/// <summary>
		///     Returns true if <paramref name="typeSymbol" /> is an Anonymous type, otherwise false.
		/// </summary>
		public static bool IsAnonymousType(this ITypeSymbol typeSymbol)
		{
			return typeSymbol?.IsAnonymousType == true;
		}

		/// <summary>
		///     Returns true if this <see cref="ITypeSymbol" /> is nested within another type, otherwise false.
		/// </summary>
		public static bool IsNested(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.ContainingType != null;
		}

		/// <summary>
		///     Returns this and all base types this derives from.
		/// </summary>
		public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
		{
			var current = type;
			while (current != null)
			{
				yield return current;
				current = current.BaseType;
			}
		}

		/// <summary>
		///     Returns all base types this derives from.
		/// </summary>
		public static IEnumerable<INamedTypeSymbol> GetBaseTypes(this ITypeSymbol type)
		{
			var current = type.BaseType;
			while (current != null)
			{
				yield return current;
				current = current.BaseType;
			}
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> inherits from a type whose fully-qualified name matches
		/// the <typeparamref name="T"/> type.
		/// </summary>
		public static bool InheritsFrom<T>(this ITypeSymbol typeSymbol)
		{
			var type = typeof(T);
			var fullTypeName = type.GetFullTypeName();

			return typeSymbol.InheritsFrom(fullTypeName);
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> inherits from a type whose fully-qualified name matches
		/// <paramref name="fullTypeName"/>.
		/// </summary>
		public static bool InheritsFrom(this ITypeSymbol typeSymbol, string fullTypeName)
		{
			var result = false;
			foreach (var baseType in typeSymbol.GetBaseTypes())
			{
				if (fullTypeName == baseType.GetFullTypeName())
				{
					result = true;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> inherits from a type or is this type whose fully-qualified
		/// name matches the <typeparamref name="T"/> type.
		/// </summary>
		public static bool InheritsFromOrIs<T>(this ITypeSymbol typeSymbol)
		{
			var type = typeof(T);
			var fullTypeName = type.GetFullTypeName();

			return typeSymbol.InheritsFromOrIs(fullTypeName);
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> inherits from a type or is this type whose fully-qualified
		/// name matches <paramref name="fullTypeName"/>.
		/// </summary>
		public static bool InheritsFromOrIs(this ITypeSymbol typeSymbol, string fullTypeName)
		{
			var result = false;
			foreach (var baseType in typeSymbol.GetBaseTypesAndThis())
			{
				if (fullTypeName == baseType.GetFullTypeName())
				{
					result = true;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Return all <see cref="ITypeSymbol"/> instances that contain this <see cref="ITypeSymbol"/> and this.
		/// </summary>
		public static IEnumerable<ITypeSymbol> GetContainingTypesAndThis(this ITypeSymbol typeSymbol)
		{
			var current = typeSymbol;
			while (current != null)
			{
				yield return current;
				current = current.ContainingType;
			}
		}

		/// <summary>
		/// Return all <see cref="ITypeSymbol"/> instances that contain this <see cref="ITypeSymbol"/>.
		/// </summary>
		public static IEnumerable<INamedTypeSymbol> GetContainingTypes(this ITypeSymbol type)
		{
			var current = type.ContainingType;
			while (current != null)
			{
				yield return current;
				current = current.ContainingType;
			}
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is a <see cref="List{T}"/>. If true,
		/// <paramref name="genericElementTypeSymbol"/> will be initialized with the list's generic type value.
		/// </summary>
		public static bool IsList(this ITypeSymbol typeSymbol, out ITypeSymbol genericElementTypeSymbol)
		{
			genericElementTypeSymbol = null;

			var isList = typeSymbol.Name == "List" &&
			             typeSymbol.ContainingNamespace.ToString() == "System.Collections.Generic";

			if (isList)
			{
				genericElementTypeSymbol = typeSymbol.GetTypeArguments()[0];
			}

			return isList;
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is a <see cref="Dictionary{TKey, TValue}"/>. If true,
		/// <paramref name="keyTypeSymbol"/> and <paramref name="valueTypeSymbol"/> will be initialized with the
		/// dictionary's generic key and type values.
		/// </summary>
		public static bool IsDictionary(
			this ITypeSymbol typeSymbol,
			out ITypeSymbol keyTypeSymbol,
			out ITypeSymbol valueTypeSymbol)
		{
			keyTypeSymbol = null;
			valueTypeSymbol = null;

			var isDictionary = typeSymbol.Name == "Dictionary" &&
			                   typeSymbol.ContainingNamespace.ToString() == "System.Collections.Generic";

			if (isDictionary)
			{
				var typeParams = typeSymbol.GetTypeArguments();
				keyTypeSymbol = typeParams[0];
				valueTypeSymbol = typeParams[1];
			}

			return isDictionary;
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is a <see cref="Array"/>. If true,
		/// <paramref name="elementTypeSymbol"/> will be initialized with the array's element type value.
		/// </summary>
		public static bool IsArray(this ITypeSymbol typeSymbol, out ITypeSymbol elementTypeSymbol)
		{
			elementTypeSymbol = null;

			var isArray = typeSymbol.IsArrayType();
			if (isArray)
			{
				elementTypeSymbol = typeSymbol.GetArrayElementType();
			}

			return isArray;
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is or derives from <see cref="Attribute"/>.
		/// </summary>
		/// <param name="typeSymbol"></param>
		/// <returns></returns>
		public static bool IsAttribute(this ITypeSymbol typeSymbol)
		{
			for (var b = typeSymbol.BaseType; b != null; b = b.BaseType)
			{
				if (b.MetadataName == nameof(Attribute) &&
				    b.ContainingType == null &&
				    b.ContainingNamespace != null &&
				    b.ContainingNamespace.Name == nameof(System) &&
				    b.ContainingNamespace.ContainingNamespace != null &&
				    b.ContainingNamespace.ContainingNamespace.IsGlobalNamespace)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Returns true if this <paramref name="typeSymbol" /> has <see cref="Attribute" /> with
		/// <paramref name="attributeTypeName"/>.
		/// </summary>
		/// <param name="canInherit">If true, all base classes of any attributes will be checked and compared for
		/// equality against <paramref name="attributeTypeName"/>, otherwise only the current attribute type will be
		/// checked.</param>
		public static bool HasAttribute(
			this ITypeSymbol typeSymbol,
			string attributeTypeName,
			bool canInherit = false)
		{
			return typeSymbol.GetAttributes().HasAttribute(attributeTypeName, canInherit);
		}

		/// <summary>
		///     Returns true if this <paramref name="typeSymbol" /> has <see cref="Attribute" />-derived type
		///     <typeparamref name="T" />.
		/// </summary>
		/// <exception cref="Exception">
		///     <typeparamref name="T" /> must be an attribute, otherwise an assertion
		///     will be thrown.
		/// </exception>
		/// <param name="canInherit">If true, all base classes of any attributes will be checked and compared for
		/// equality against <paramref name="typeSymbol"/>, otherwise only the current attribute type will be
		/// checked.</param>
		public static bool HasAttribute<T>(this ITypeSymbol typeSymbol, bool canInherit = false)
		{
			if (!typeof(Attribute).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException("T must be assignable to Attribute.");
			}

			return typeSymbol.HasAttribute(typeof(T).Name, canInherit);
		}

		/// <summary>
		/// Returns all <see cref="AttributeData"/> decorated on this <see cref="ITypeSymbol"/> where the
		/// attribute class's name matches <paramref name="attributeTypeName"/>.
		/// </summary>
		/// <param name="canInherit">If true, all base classes of any attributes will be checked and compared for
		/// equality against <paramref name="attributeTypeName"/>, otherwise only the current attribute type will be
		/// checked.</param>
		public static IEnumerable<AttributeData> GetAttributes(
			this ITypeSymbol typeSymbol,
			string attributeTypeName,
			bool canInherit = false)
		{
			return typeSymbol.GetAttributes().GetAttributes(attributeTypeName, canInherit);
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is a nullable type or returns a nullable value, otherwise false.
		/// </summary>
		public static bool IsNullableType(this ITypeSymbol type)
		{
			var original = type.OriginalDefinition;
			return original.SpecialType == SpecialType.System_Nullable_T;
		}

		/// <summary>
		/// Returns the underlying type of the nullable, if present.
		/// </summary>
		public static ITypeSymbol GetNullableUnderlyingType(this ITypeSymbol typeSymbol)
		{
			if (!IsNullableType(typeSymbol))
			{
				return null;
			}

			return ((INamedTypeSymbol)typeSymbol).TypeArguments[0];
		}

		/// <summary>
		///     Gets all base classes.
		/// </summary>
		/// <returns>The all base classes.</returns>
		/// <param name="type">Type.</param>
		public static IEnumerable<INamedTypeSymbol> GetAllBaseClasses(this INamedTypeSymbol type,
			bool includeSuperType = false)
		{
			if (!includeSuperType)
				type = type.BaseType;
			while (type != null)
			{
				yield return type;
				type = type.BaseType;
			}
		}

		/// <summary>
		///     Gets all base classes and interfaces.
		/// </summary>
		/// <returns>All classes and interfaces.</returns>
		/// <param name="type">Type.</param>
		public static IEnumerable<INamedTypeSymbol> GetAllBaseClassesAndInterfaces(this INamedTypeSymbol type,
			bool includeSuperType = false)
		{
			if (!includeSuperType)
				type = type.BaseType;
			var curType = type;
			while (curType != null)
			{
				yield return curType;
				curType = curType.BaseType;
			}

			foreach (var inter in type.AllInterfaces) yield return inter;
		}

		/// <summary>
		///     Determines if derived from baseType. Includes itself and all base classes, but does not include interfaces.
		/// </summary>
		/// <returns><c>true</c> if is derived from class the specified type baseType; otherwise, <c>false</c>.</returns>
		/// <param name="type">Type.</param>
		/// <param name="baseType">Base type.</param>
		public static bool IsDerivedFromClass(this INamedTypeSymbol type, INamedTypeSymbol baseType)
		{
			for (; type != null; type = type.BaseType)
			{
				if (SymbolEqualityComparer.Default.Equals(type, baseType))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		///     Determines if derived from baseType. Includes itself, all base classes and all interfaces.
		/// </summary>
		/// <returns><c>true</c> if is derived from the specified type baseType; otherwise, <c>false</c>.</returns>
		/// <param name="type">Type.</param>
		/// <param name="baseType">Base type.</param>
		public static bool IsDerivedFromClassOrInterface(this INamedTypeSymbol type, INamedTypeSymbol baseType)
		{
			for (; type != null; type = type.BaseType)
			{
				if (SymbolEqualityComparer.Default.Equals(type, baseType))
				{
					return true;
				}
			}

			// And interfaces
			foreach (var inter in type.AllInterfaces)
			{
				if (SymbolEqualityComparer.Default.Equals(inter,baseType))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is an numeric type, otherwise false.
		/// </summary>
		public static bool IsNumericType(this ITypeSymbol type)
		{
			if (type != null)
				switch (type.SpecialType)
				{
					case SpecialType.System_Byte:
					case SpecialType.System_SByte:
					case SpecialType.System_Int16:
					case SpecialType.System_UInt16:
					case SpecialType.System_Int32:
					case SpecialType.System_UInt32:
					case SpecialType.System_Int64:
					case SpecialType.System_UInt64:
					case SpecialType.System_Single:
					case SpecialType.System_Double:
					case SpecialType.System_Decimal:
						return true;
				}

			return false;
		}

		/// <summary>
		///     Returns the full-type name of this <paramref name="typeSymbol" />.
		/// </summary>
		public static string GetFullTypeName(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.ToString().Replace("*", string.Empty);
		}

		/// <summary>
		///     Returns true if this <paramref name="typeSymbol" /> is a generic type, otherwise false.
		/// </summary>
		public static bool IsGenericType(this ITypeSymbol typeSymbol)
		{
			if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
			{
				return namedTypeSymbol.IsGenericType;
			}

			return typeSymbol.BaseType != null && typeSymbol.BaseType.IsGenericType;
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> has a default constructor, otherwise false.
		/// </summary>
		public static bool HasDefaultConstructor(this ITypeSymbol typeSymbol)
		{
			return typeSymbol
				.GetMembers()
				.Any(x => x.IsConstructor() && x.GetParameters().Length == 0);
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> has a constructor that accepts a single parameter of the
		/// same type, otherwise false.
		/// </summary>
		public static bool HasCopyConstructor(this ITypeSymbol typeSymbol)
		{
			return typeSymbol
				.GetMembers()
				.Any(x =>
					x.IsConstructor() &&
					x.GetParameters().Length == 1 &&
					SymbolEqualityComparer.Default.Equals(x.GetParameters()[0].Type, typeSymbol));
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> implements <see cref="ICloneable"/>, otherwise false.
		/// </summary>
		public static bool IsCloneable(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.ImplementsInterface<ICloneable>();
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is a mutable reference type, otherwise false.
		/// </summary>
		public static bool IsMutableReferenceType(this ITypeSymbol typeSymbol)
		{
			return !typeSymbol.IsValueType && typeSymbol.GetFullTypeName() != "string";
		}

		/// <summary>
		///     Returns a safe-readable version of a short type name, without generic or array characters.
		/// </summary>
		public static string GetHumanReadableName(this ITypeSymbol typeSymbol)
		{
			var result = typeSymbol.Name.UppercaseFirst();
			if (typeSymbol.IsArrayType())
			{
				var elementType = typeSymbol.GetArrayElementType();
				result = string.Format(CodeGenerationConstants.ARRAY_SHORT_NAME, elementType.GetTypeNameOrAlias());
			}
			else if (typeSymbol.IsGenericType())
			{
				var namedTypeSymbol = (INamedTypeSymbol) typeSymbol;
				var leftChevronIndex = result.IndexOf(CodeGenerationConstants.LEFT_CHEVRON_CHAR);
				if (leftChevronIndex > 0) result = result.Remove(leftChevronIndex);

				var genericTypeParameters = namedTypeSymbol.TypeArguments;
				foreach (var genericTypeParameter in genericTypeParameters)
					result += genericTypeParameter.GetHumanReadableName();
			}

			return result;
		}

		/// <summary>
		///     Returns the short type name or alias for this <paramref name="typeSymbol" />.
		/// </summary>
		public static string GetTypeNameOrAlias(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.Name.GetTypeNameOrAlias();
		}

		/// <summary>
		///     Returns the array element <see cref="ITypeSymbol" /> for this instance. If not an array type, an exception
		///     will be thrown.
		/// </summary>
		public static ITypeSymbol GetArrayElementType(this ITypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsArrayType())
			{
				const string EXCEPTION_MSG = "TypeSymbol [{0}] must be an array type in order to retrieve an " +
				                             "array element type.";

				throw new ArgumentException(
					string.Format(EXCEPTION_MSG, typeSymbol),
					nameof(typeSymbol));
			}

			var arrayTypeSymbol = (IArrayTypeSymbol) typeSymbol;
			return arrayTypeSymbol.ElementType;
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> supports using a collection initializer, otherwise false.
		/// </summary>
		public static bool CanSupportCollectionInitializer(this ITypeSymbol typeSymbol)
		{
			if (typeSymbol.AllInterfaces.Any(i => i.SpecialType == SpecialType.System_Collections_IEnumerable))
			{
				var curType = typeSymbol;
				while (curType != null)
				{
					if (HasAddMethod(curType))
						return true;
					curType = curType.BaseType;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> has the <see cref="ICollection{T}.Add"/> method, otherwise
		/// false.
		/// </summary>
		private static bool HasAddMethod(ITypeSymbol typeSymbol)
		{
			return typeSymbol
				.GetMembers(WellKnownMemberNames.CollectionInitializerAddMethodName)
				.OfType<IMethodSymbol>().Any(m => m.Parameters.Any());
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is an enum, otherwise false.
		/// </summary>
		public static bool IsEnumType(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.IsValueType && typeSymbol.TypeKind == TypeKind.Enum;
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is an <see cref="IEnumerable{T}"/>, otherwise false.
		/// </summary>
		public static bool IsIEnumerable(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.ImplementsSpecialTypeInterface(SpecialType.System_Collections_IEnumerable);
		}
	}
}
