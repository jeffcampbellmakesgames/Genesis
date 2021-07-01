using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genesis.Plugin
{
	/// <summary>
	/// A wrapper for <see cref="INamedTypeSymbol"/> that caches it's commonly used members that load frequently.
	/// </summary>
	internal class CachedNamedTypeSymbol : ICachedNamedTypeSymbol
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
		/// Constructor for <see cref="CachedNamedTypeSymbol"/>.
		/// </summary>
		public CachedNamedTypeSymbol(INamedTypeSymbol namedTypeSymbol)
		{
			NamedTypeSymbol = namedTypeSymbol;
		}

		/// <inheritdoc />
		public bool ImplementsInterface<T>()
		{
			if (!typeof(T).IsInterface)
			{
				throw new ArgumentException("T must be an Interface.");
			}

			var interfaceName = typeof(T).Name;

			return ImplementsInterface(interfaceName);
		}

		/// <inheritdoc />
		public bool ImplementsInterface(string interfaceTypeName)
		{
			return NamedTypeSymbol.Name != interfaceTypeName &&
			       InterfaceTypeSymbols.Any(interfaceTypeSymbol => interfaceTypeSymbol.Name == interfaceTypeName);
		}

		/// <inheritdoc />
		public IEnumerable<AttributeData> GetAttributes(string attributeTypeName, bool canInherit = false)
		{
			return AttributeData.GetAttributes(attributeTypeName, canInherit);
		}

		/// <inheritdoc />
		public bool HasAttribute<T>(bool canInherit = false)
		{
			if (!typeof(Attribute).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException("T must be assignable to Attribute.");
			}

			return HasAttribute(typeof(T).Name, canInherit);
		}

		/// <inheritdoc />
		public bool HasAttribute(string attributeTypeName, bool canInherit = false)
		{
			return AttributeData.HasAttribute(attributeTypeName, canInherit);
		}

		#region Static Helpers

		/// <summary>
		/// Factory method for creating a <see cref="CachedNamedTypeSymbol"/> instance.
		/// </summary>
		/// <param name="namedTypeSymbol"></param>
		/// <returns></returns>
		public static CachedNamedTypeSymbol Create(INamedTypeSymbol namedTypeSymbol) =>
			new CachedNamedTypeSymbol(namedTypeSymbol);

		#endregion
	}
}
