using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genesis.Plugin
{
	/// <summary>
	/// A wrapper for <see cref="INamedTypeSymbol"/> that caches it's commonly used members that load frequently.
	/// </summary>
	public class NamedTypeSymbolInfo
	{
		/// <summary>
		/// The raw <see cref="INamedTypeSymbol"/>.
		/// </summary>
		public INamedTypeSymbol NamedTypeSymbol { get; }

		/// <summary>
		/// All <see cref="AttributeData"/> decorated on this <see cref="NamedTypeSymbol"/>.  Lazy-loaded.
		/// </summary>
		public IEnumerable<AttributeData> AttributeData
		{
			get { return _attributeData ??= NamedTypeSymbol.GetAttributes(); }
		}

		/// <summary>
		/// All interface <see cref="INamedTypeSymbol"/> instances that this <see cref="NamedTypeSymbol"/> and it's
		/// base types implement. Lazy-loaded.
		/// </summary>
		public IEnumerable<INamedTypeSymbol> InterfaceTypeSymbols
		{
			get { return _interfaceTypeSymbols ??= NamedTypeSymbol.AllInterfaces; }
		}

		/// <summary>
		/// All base <see cref="ITypeSymbol"/> instances of <see cref="NamedTypeSymbol"/>, including itself.
		/// Lazy-loaded.
		/// </summary>
		public IEnumerable<ITypeSymbol> BaseTypesAndThis
		{
			get { return _baseTypesAndThis ??= NamedTypeSymbol.GetBaseTypesAndThis(); }
		}

		/// <summary>
		/// All public <see cref="ISymbol"/> members of <see cref="NamedTypeSymbol"/>. Lazy-loaded.
		/// </summary>
		public IEnumerable<ISymbol> AllPublicMembers
		{
			get { return _allPublicMembers ??= NamedTypeSymbol.GetAllMembers(); }
		}

		/// <summary>
		/// Full type name for <see cref="NamedTypeSymbol"/>. Lazy-loaded.
		/// </summary>
		public string FullTypeName
		{
			get
			{
				if (string.IsNullOrEmpty(_fullTypeName))
				{
					_fullTypeName = NamedTypeSymbol.GetFullTypeName();
				}

				return _fullTypeName;
			}
		}

		/// <summary>
		/// Full type name for <see cref="NamedTypeSymbol"/>. Lazy-loaded.
		/// </summary>
		public string TypeName
		{
			get
			{
				if (string.IsNullOrEmpty(_typeName))
				{
					_typeName = NamedTypeSymbol.Name;
				}

				return _typeName;
			}
		}

		private IEnumerable<AttributeData> _attributeData;
		private IEnumerable<INamedTypeSymbol> _interfaceTypeSymbols;
		private IEnumerable<ITypeSymbol> _baseTypesAndThis;
		private IEnumerable<ISymbol> _allPublicMembers;
		private string _fullTypeName;
		private string _typeName;

		/// <summary>
		/// Constructor for <see cref="NamedTypeSymbolInfo"/>.
		/// </summary>
		public NamedTypeSymbolInfo(INamedTypeSymbol namedTypeSymbol)
		{
			NamedTypeSymbol = namedTypeSymbol;
		}

		/// <summary>
		/// Returns true if this <see cref="NamedTypeSymbol"/> is decorated with interface type
		/// <typeparamref name="T"/>.
		/// </summary>
		/// <exception cref="ArgumentException">
		///     <typeparamref name="T" /> must be an interface, otherwise an exception will be thrown.
		/// </exception>
		public bool ImplementsInterface<T>()
		{
			if (!typeof(T).IsInterface)
			{
				throw new ArgumentException("T must be an Interface.");
			}

			var interfaceName = typeof(T).Name;

			return ImplementsInterface(interfaceName);
		}

		/// <summary>
		///     Returns true if <see cref="NamedTypeSymbol" /> implements an interface matching
		/// <paramref name="interfaceTypeName" />.
		/// </summary>
		public bool ImplementsInterface(string interfaceTypeName)
		{
			return NamedTypeSymbol.Name != interfaceTypeName &&
			       InterfaceTypeSymbols.Any(interfaceTypeSymbol => interfaceTypeSymbol.Name == interfaceTypeName);
		}

		/// <summary>
		/// Returns all <see cref="Microsoft.CodeAnalysis.AttributeData"/> for <see cref="NamedTypeSymbol"/> where
		/// the attribute class's name matches <paramref name="attributeTypeName"/>.
		/// </summary>
		public IEnumerable<AttributeData> GetAttributes(string attributeTypeName)
		{
			return AttributeData.GetAttributes(attributeTypeName);
		}

		/// <summary>
		///     Returns true if <see cref="NamedTypeSymbol"/> has <see cref="Attribute" />-derived type
		///     <typeparamref name="T" />.
		/// </summary>
		/// <exception cref="Exception">
		///     <typeparamref name="T" /> must be an attribute, otherwise an assertion
		///     will be thrown.
		/// </exception>
		public bool HasAttribute<T>()
		{
			if (!typeof(Attribute).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException("T must be assignable to Attribute.");
			}

			return HasAttribute(typeof(T).Name);
		}

		/// <summary>
		///     Returns true if <see cref="NamedTypeSymbol" /> has an
		/// <see cref="Microsoft.CodeAnalysis.AttributeData" /> with <paramref name="attributeTypeName"/>.
		/// </summary>
		public bool HasAttribute(string attributeTypeName)
		{
			return AttributeData.HasAttribute(attributeTypeName);
		}

		#region Static Helpers

		/// <summary>
		/// Factory method for creating a <see cref="NamedTypeSymbolInfo"/> instance.
		/// </summary>
		/// <param name="namedTypeSymbol"></param>
		/// <returns></returns>
		public static NamedTypeSymbolInfo Create(INamedTypeSymbol namedTypeSymbol) =>
			new NamedTypeSymbolInfo(namedTypeSymbol);

		#endregion
	}
}
