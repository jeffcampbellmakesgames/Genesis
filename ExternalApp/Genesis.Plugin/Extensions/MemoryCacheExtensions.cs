using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	/// Helper methods for <see cref="IMemoryCache"/>.
	/// </summary>
	public static class MemoryCacheExtensions
	{
		/// <summary>
		/// Adds the <see cref="Solution"/> to the memory cache.
		/// </summary>
		public static void AddSolution(this IMemoryCache memoryCache, Solution solution)
		{
			memoryCache.Add(MemoryCacheConstants.CODE_ANALYSIS_SOLUTION_KEY, solution);
		}

		/// <summary>
		/// Returns a <see cref="Solution"/> stored in this <see cref="IMemoryCache"/> with key
		/// <see cref="MemoryCacheConstants.CODE_ANALYSIS_SOLUTION_KEY"/>.
		/// </summary>
		public static Solution GetSolution(this IMemoryCache memoryCache)
		{
			return memoryCache.Get<Solution>(MemoryCacheConstants.CODE_ANALYSIS_SOLUTION_KEY);
		}

		/// <summary>
		/// Adds the read-only collection of <paramref name="typeSymbols" /> to the memory cache.
		/// </summary>
		public static void AddNamedTypeSymbols(this IMemoryCache memoryCache, IReadOnlyList<INamedTypeSymbol> typeSymbols)
		{
			memoryCache.Add(MemoryCacheConstants.CODE_ANALYSIS_TYPE_SYMBOLS_KEY, typeSymbols);
		}

		/// <summary>
		/// Returns the read-only collection of <see cref="INamedTypeSymbol"/> instances from the memory cache.
		/// </summary>
		/// <param name="memoryCache"></param>
		/// <returns></returns>
		public static IReadOnlyList<INamedTypeSymbol> GetNamedTypeSymbols(this IMemoryCache memoryCache)
		{
			return memoryCache.Get<IReadOnlyList<INamedTypeSymbol>>(MemoryCacheConstants.CODE_ANALYSIS_TYPE_SYMBOLS_KEY);
		}
	}
}
