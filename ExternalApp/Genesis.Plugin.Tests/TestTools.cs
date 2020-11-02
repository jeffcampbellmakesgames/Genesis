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
using System.IO;
using System.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Genesis.Plugin.Tests
{
	/// <summary>
	/// Helper methods for unit-testing Roslyn symbols.
	/// </summary>
	public static class TestTools
	{
		private static IReadOnlyList<INamedTypeSymbol> _ALL_NAMED_TYPE_SYMBOLS;

		private const string SOLUTION_PATH = @"../../../../UnityProjectFixtures/UnityProjectFixtures.sln";

		/// <summary>
		/// Returns a read-only collection of <see cref="INamedTypeSymbol"/> instances in the Unity fixture project.
		/// </summary>
		/// <returns></returns>
		public static IReadOnlyList<INamedTypeSymbol> GetAllFixtureTypeSymbols()
		{
			if (_ALL_NAMED_TYPE_SYMBOLS != null)
			{
				return _ALL_NAMED_TYPE_SYMBOLS;
			}

			Solution solution;
			MSBuildLocator.RegisterDefaults();
			using (var workspace = MSBuildWorkspace.Create())
			{
				var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), SOLUTION_PATH));

				solution = workspace.OpenSolutionAsync(path).Result;
			}

			return _ALL_NAMED_TYPE_SYMBOLS = CodeAnalysisTools.FindAllTypes(solution);
		}

		/// <summary>
		/// Returns a simple C# <see cref="ITypeSymbol"/>.
		/// </summary>
		public static ITypeSymbol GetSimpleTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "CreatureBehaviour");
		}

		/// <summary>
		/// Returns a generic C# <see cref="ITypeSymbol"/>.
		/// </summary>
		public static ITypeSymbol GetOpenGenericTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "GenericBehaviour");
		}

		/// <summary>
		/// Returns a generic C# <see cref="ITypeSymbol"/>.
		/// </summary>
		public static ITypeSymbol GetClosedGenericTypeSymbol()
		{
			var creatureTypeSymbol = GetAllFixtureTypeSymbols().First(x => x.Name == "CreatureType");
			var arrayTypeSymbol = (ITypeSymbol)creatureTypeSymbol.GetAttributes()
				.First(x => x.ConstructorArguments[0].Value.ToString() == "System.Collections.Generic.List<UnityEngine.GameObject>")
				.ConstructorArguments[0]
				.Value;

			return arrayTypeSymbol;
		}

		/// <summary>
		/// Returns an array C# <see cref="ITypeSymbol"/>.
		/// </summary>
		public static ITypeSymbol GetArrayTypeSymbol()
		{
			var creatureTypeSymbol = GetAllFixtureTypeSymbols().First(x => x.Name == "CreatureType");
			var arrayTypeSymbol = (ITypeSymbol)creatureTypeSymbol.GetAttributes()
				.First(x => x.ConstructorArguments[0].Value.ToString() == "UnityEngine.GameObject[]")
				.ConstructorArguments[0]
				.Value;

			return arrayTypeSymbol;
		}
	}
}
