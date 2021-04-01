using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	/// Helper methods for <see cref="INamedTypeSymbol"/>.
	/// </summary>
	public static class INamedTypeSymbolExtensions
	{
		/// <summary>
		/// Returns true if this <paramref name="namedTypeSymbol"/> has <see cref="Attribute"/>-derived type
		/// <typeparamref name="T"/>.
		/// </summary>
		/// <exception cref="AssertionException"><typeparamref name="T"/> must be an attribute, otherwise an assertion
		/// will be thrown.</exception>
		public static bool HasAttribute<T>(this INamedTypeSymbol namedTypeSymbol)
		{
			Debug.Assert(typeof(T).IsAssignableFrom(typeof(Attribute)));

			return namedTypeSymbol.GetAttributes().Any(attr =>
				attr.AttributeClass != null &&
				attr.AttributeClass.Name == nameof(T));
		}

		/// <summary>
		/// Returns an enumerable of constructor arguments for any <see cref="Attribute"/> instances of type
		/// <typeparamref name="T"/>, if any.
		/// </summary>
		/// <exception cref="AssertionException"><typeparamref name="T"/> must be an attribute, otherwise an assertion
		/// will be thrown.</exception>
		public static IEnumerable<ImmutableArray<TypedConstant>> GetAttributeConstructorArguments<T>(this INamedTypeSymbol namedTypeSymbol)
		{
			Debug.Assert(typeof(T).IsAssignableFrom(typeof(Attribute)));

			return namedTypeSymbol.GetAttributes()
				.Where(attr =>
					attr.AttributeClass != null &&
					attr.AttributeClass.Name == nameof(T))
				.Select(attr => attr.ConstructorArguments);
		}
	}
}
