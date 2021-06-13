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
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	///     Helper methods for <see cref="IMemoryCache" />.
	/// </summary>
	public static class MemoryCacheExtensions
	{
		/// <summary>
		///     Adds the <see cref="Solution" /> to the memory cache.
		/// </summary>
		public static void AddSolution(this IMemoryCache memoryCache, Solution solution)
		{
			memoryCache.Add(MemoryCacheConstants.CODE_ANALYSIS_SOLUTION_KEY, solution);
		}

		/// <summary>
		///     Returns a <see cref="Solution" /> stored in this <see cref="IMemoryCache" /> with key
		///     <see cref="MemoryCacheConstants.CODE_ANALYSIS_SOLUTION_KEY" />.
		/// </summary>
		public static Solution GetSolution(this IMemoryCache memoryCache)
		{
			return memoryCache.Get<Solution>(MemoryCacheConstants.CODE_ANALYSIS_SOLUTION_KEY);
		}

		/// <summary>
		///     Adds the read-only collection of <paramref name="typeSymbols" /> to the memory cache.
		/// </summary>
		public static void AddNamedTypeSymbols(this IMemoryCache memoryCache,
			IReadOnlyList<ICachedNamedTypeSymbol> typeSymbols)
		{
			memoryCache.Add(MemoryCacheConstants.CODE_ANALYSIS_TYPE_SYMBOLS_KEY, typeSymbols);
		}

		/// <summary>
		///     Returns the read-only collection of <see cref="INamedTypeSymbol" /> instances from the memory cache.
		/// </summary>
		public static IReadOnlyList<ICachedNamedTypeSymbol> GetNamedTypeSymbols(this IMemoryCache memoryCache)
		{
			return memoryCache.Get<IReadOnlyList<ICachedNamedTypeSymbol>>(MemoryCacheConstants
				.CODE_ANALYSIS_TYPE_SYMBOLS_KEY);
		}
	}
}
