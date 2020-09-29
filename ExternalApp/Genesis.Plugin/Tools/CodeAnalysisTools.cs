using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Serilog;

namespace Genesis.Plugin
{
	/// <summary>
	/// Helper methods for code analysis.
	/// </summary>
	public static class CodeAnalysisTools
	{
		private static readonly ILogger LOGGER = Log.ForContext(typeof(CodeAnalysisTools));

		/// <summary>
		/// Returns a read-only collection of all <see cref="INamedTypeSymbol"/> instances from the
		/// <paramref name="solution"/>.
		/// </summary>
		public static IReadOnlyList<INamedTypeSymbol> FindAllTypes(Solution solution)
		{
			var allTypeSymbols = new List<INamedTypeSymbol>();
			if (solution == null)
			{
				LOGGER.Verbose("No solution found, skipping Roslyn parsing");
			}
			else
			{
				LOGGER.Verbose("Roslyn solution found, beginning parsing.");

				// Collect all type symbols from each project and set the resultant collection into the memory cache
				foreach (var project in solution.Projects)
				{
					LOGGER.Verbose("Inspecting project {ProjectName}.", project.Name);

					var compilation = project.GetCompilationAsync().Result;
					var namedTypeSymbols = compilation
						.GetSymbolsWithName(x => true, SymbolFilter.Type)
						.OfType<ITypeSymbol>()
						.OfType<INamedTypeSymbol>()
						.ToArray();

					LOGGER.Verbose("Found {ProjectTypeSymbolCount} in {ProjectName}.", namedTypeSymbols.Length, project.Name);

					allTypeSymbols.AddRange(namedTypeSymbols);
				}

				LOGGER.Verbose("Found a total of {TypeSymbolsCount}.", allTypeSymbols.Count);
			}

			return allTypeSymbols;
		}
	}
}
