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

using System.Collections.Generic;
using System.Linq;
using Genesis.Shared;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	///     Helper methods for <see cref="AssembliesConfig" />
	/// </summary>
	public static class AssembliesConfigExtensions
	{
		/// <summary>
		///     If <paramref name="config" /> is set to whitelist assemblies, it filters the superset of
		///     <paramref name="namedTypeSymbols" /> to only those contained in assemblies
		///     defined in this config.
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

		/// <summary>
		///     If <paramref name="config" /> is set to whitelist assemblies, it filters the superset of
		///     <paramref name="namedTypeSymbolInfo" /> to only those contained in assemblies
		///     defined in this config.
		/// </summary>
		public static IReadOnlyList<ICachedNamedTypeSymbol> FilterTypeSymbols(
			this AssembliesConfig config,
			IReadOnlyList<ICachedNamedTypeSymbol> namedTypeSymbolInfo)
		{
			if (config.DoUseWhitelistOfAssemblies)
			{
				var whitelistedAssemblies = config.WhiteListedAssemblies.ToList();
				var filteredList = new List<ICachedNamedTypeSymbol>();
				for (var i = namedTypeSymbolInfo.Count - 1; i >= 0; i--)
				{
					var symbolInfo = namedTypeSymbolInfo[i];
					if (whitelistedAssemblies.Contains(symbolInfo.NamedTypeSymbol.ContainingAssembly.Name))
					{
						filteredList.Add(symbolInfo);
					}
				}

				return filteredList;
			}

			return namedTypeSymbolInfo;
		}
	}
}
