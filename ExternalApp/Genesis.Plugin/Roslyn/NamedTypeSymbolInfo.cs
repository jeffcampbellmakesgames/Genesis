using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

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
		/// All <see cref="AttributeData"/> decorated on this <see cref="NamedTypeSymbol"/>
		/// </summary>
		public IEnumerable<AttributeData> AttributeData { get; }

		/// <summary>
		/// All interface <see cref="INamedTypeSymbol"/> instances that this <see cref="NamedTypeSymbol"/> and it's
		/// base types implement.
		/// </summary>
		public IEnumerable<INamedTypeSymbol> InterfaceTypeSymbols { get; set; }
		public IEnumerable<ITypeSymbol> BaseTypesAndThis { get; set; }

		/// <summary>
		/// Constructor for <see cref="NamedTypeSymbolInfo"/> that caches all commonly used members on creation.
		/// </summary>
		/// <param name="namedTypeSymbol"></param>
		public NamedTypeSymbolInfo(INamedTypeSymbol namedTypeSymbol)
		{
			NamedTypeSymbol = namedTypeSymbol;
			AttributeData = namedTypeSymbol.GetAttributes();
			BaseTypesAndThis = namedTypeSymbol.GetBaseTypesAndThis();
			InterfaceTypeSymbols = BaseTypesAndThis.SelectMany(x => x.AllInterfaces).Distinct();
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
		/// Returns all <see cref="Microsoft.CodeAnalysis.AttributeData"/> for this <see cref="NamedTypeSymbol"/> where
		/// the attribute class's name matches <paramref name="attributeTypeName"/>.
		/// </summary>
		public IEnumerable<AttributeData> GetAttributes(string attributeTypeName)
		{
			return AttributeData.GetAttributes(attributeTypeName);
		}

		/// <summary>
		///     Returns true if this <see cref="NamedTypeSymbol" /> has an
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
