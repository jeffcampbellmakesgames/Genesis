using System.Collections.Generic;
using System.Linq;
using Genesis.Shared;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	/// Helper methods for <see cref="AssembliesConfig"/>
	/// </summary>
	internal static class AssembliesConfigExtensions
	{
		/// <summary>
		/// If <paramref name="config"/> is set to whitelist assemblies, it filters the superset of <paramref name="namedTypeSymbols"/> to only those contained in assemblies
		/// defined in this config.
		/// </summary>
		public static IReadOnlyList<INamedTypeSymbol> FilterTypeSymbols(
			this AssembliesConfig config,
			IReadOnlyList<INamedTypeSymbol> namedTypeSymbols)
		{
			if (config.DoUseWhitelistOfAssemblies)
			{
				var whitelistedAssemblies = config.WhiteListedAssemblies.ToList();
				var filteredList = new List<INamedTypeSymbol>();
				for (var i = namedTypeSymbols.Count - 1; i >= 0; i--)
				{
					var namedTypeSymbol = namedTypeSymbols[i];
					if (whitelistedAssemblies.Contains(namedTypeSymbol.ContainingAssembly.Name))
					{
						filteredList.Add(namedTypeSymbol);
					}
				}

				return filteredList;
			}

			return namedTypeSymbols;
		}
	}
}
