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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.CodeAnalysis;
using Serilog;

namespace Genesis.Plugin
{
	/// <summary>
	///     Helper methods for code analysis.
	/// </summary>
	public static class CodeAnalysisTools
	{
		private static readonly ILogger LOGGER = Log.ForContext(typeof(CodeAnalysisTools));

		/// <summary>
		///     Returns a read-only collection of all <see cref="INamedTypeSymbol" /> instances from the
		///     <paramref name="solution" />.
		/// </summary>
		public static async Task<IReadOnlyList<INamedTypeSymbol>> FindAllTypes(Solution solution)
		{
			var allTypeSymbols = new ConcurrentBag<INamedTypeSymbol>();
			if (solution == null)
			{
				LOGGER.Verbose("No solution found, skipping Roslyn parsing");
			}
			else
			{
				LOGGER.Verbose("Roslyn solution found, beginning parsing.");

				// Collect all type symbols from each project and set the resultant collection into the memory cache
				// Process each project in the solution
				var processProjectBlock = new ActionBlock<Project>(
					async project =>
					{
						LOGGER.Verbose("Inspecting project {ProjectName}.", project.Name);

						var compilation = await project.GetCompilationAsync();
						var namedTypeSymbols = compilation
							.GetSymbolsWithName(x => true, SymbolFilter.Type)
							.OfType<ITypeSymbol>()
							.OfType<INamedTypeSymbol>();

						foreach (var namedTypeSymbol in namedTypeSymbols)
						{
							allTypeSymbols.Add(namedTypeSymbol);
						}
					},
					new ExecutionDataflowBlockOptions
					{
						BoundedCapacity = 10000,
						MaxDegreeOfParallelism = Environment.ProcessorCount
					});

				foreach (var project in solution.Projects)
				{
					await processProjectBlock.SendAsync(project);
				}

				processProjectBlock.Complete();
				await processProjectBlock.Completion;

				LOGGER.Verbose("Found a total of {TypeSymbolsCount}.", allTypeSymbols.Count);
			}

			return allTypeSymbols.ToList();
		}
	}
}
