using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	/// Represents a cache of information about a <see cref="INamedTypeSymbol"/> instance.
	/// </summary>
	public interface ICachedNamedTypeSymbol
	{
		/// <summary>
		/// The raw <see cref="INamedTypeSymbol"/>.
		/// </summary>
		INamedTypeSymbol NamedTypeSymbol { get; }

		/// <summary>
		/// All <see cref="AttributeData"/> decorated on this <see cref="NamedTypeSymbol"/>.  Lazy-loaded.
		/// </summary>
		IEnumerable<AttributeData> AttributeData { get; }

		/// <summary>
		/// All interface <see cref="INamedTypeSymbol"/> instances that this <see cref="NamedTypeSymbol"/> and it's
		/// base types implement. Lazy-loaded.
		/// </summary>
		IEnumerable<INamedTypeSymbol> InterfaceTypeSymbols { get; }

		/// <summary>
		/// All base <see cref="ITypeSymbol"/> instances of <see cref="NamedTypeSymbol"/>, including itself.
		/// Lazy-loaded.
		/// </summary>
		IEnumerable<ITypeSymbol> BaseTypesAndThis { get; }

		/// <summary>
		/// All public <see cref="ISymbol"/> members of <see cref="NamedTypeSymbol"/>. Lazy-loaded.
		/// </summary>
		IEnumerable<ISymbol> AllPublicMembers { get; }

		/// <summary>
		/// Full type name for <see cref="NamedTypeSymbol"/>. Lazy-loaded.
		/// </summary>
		string FullTypeName { get; }

		/// <summary>
		/// Full type name for <see cref="NamedTypeSymbol"/>. Lazy-loaded.
		/// </summary>
		string TypeName { get; }

		/// <summary>
		/// Returns true if this <see cref="NamedTypeSymbol"/> is decorated with interface type
		/// <typeparamref name="T"/>.
		/// </summary>
		/// <exception cref="ArgumentException">
		///     <typeparamref name="T" /> must be an interface, otherwise an exception will be thrown.
		/// </exception>
		bool ImplementsInterface<T>();

		/// <summary>
		///     Returns true if <see cref="NamedTypeSymbol" /> implements an interface matching
		/// <paramref name="interfaceTypeName" />.
		/// </summary>
		bool ImplementsInterface(string interfaceTypeName);

		/// <summary>
		/// Returns all <see cref="Microsoft.CodeAnalysis.AttributeData"/> for <see cref="NamedTypeSymbol"/> where
		/// the attribute class's name matches <paramref name="attributeTypeName"/>.
		/// </summary>
		IEnumerable<AttributeData> GetAttributes(string attributeTypeName);

		/// <summary>
		///     Returns true if <see cref="NamedTypeSymbol"/> has <see cref="Attribute" />-derived type
		///     <typeparamref name="T" />.
		/// </summary>
		/// <exception cref="Exception">
		///     <typeparamref name="T" /> must be an attribute, otherwise an assertion
		///     will be thrown.
		/// </exception>
		bool HasAttribute<T>();

		/// <summary>
		///     Returns true if <see cref="NamedTypeSymbol" /> has an
		/// <see cref="Microsoft.CodeAnalysis.AttributeData" /> with <paramref name="attributeTypeName"/>.
		/// </summary>
		bool HasAttribute(string attributeTypeName);
	}
}
