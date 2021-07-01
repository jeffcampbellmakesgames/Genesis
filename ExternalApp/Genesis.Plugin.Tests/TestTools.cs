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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Genesis.Unity.Factory;
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
		private static IReadOnlyList<IAssemblySymbol> _ALL_ASSEMBLY_SYMBOLS;
		private static bool _HAVE_DEFAULTS_BEEN_REGISTERED;

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

			if (!_HAVE_DEFAULTS_BEEN_REGISTERED)
			{
				MSBuildLocator.RegisterDefaults();

				_HAVE_DEFAULTS_BEEN_REGISTERED = true;
			}

			using (var workspace = MSBuildWorkspace.Create())
			{
				var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), SOLUTION_PATH));
				var solution = workspace.OpenSolutionAsync(path).Result;

				_ALL_NAMED_TYPE_SYMBOLS = CodeAnalysisTools.FindAllTypes(solution).Result;
			}

			return _ALL_NAMED_TYPE_SYMBOLS;
		}

		public static IReadOnlyList<IAssemblySymbol> GetAllFixtureAssemblySymbols()
		{
			if (_ALL_ASSEMBLY_SYMBOLS != null)
			{
				return _ALL_ASSEMBLY_SYMBOLS;
			}

			if (!_HAVE_DEFAULTS_BEEN_REGISTERED)
			{
				MSBuildLocator.RegisterDefaults();

				_HAVE_DEFAULTS_BEEN_REGISTERED = true;
			}

			using (var workspace = MSBuildWorkspace.Create())
			{
				var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), SOLUTION_PATH));
				var solution = workspace.OpenSolutionAsync(path).Result;
				var assemblySymbolList = new List<IAssemblySymbol>();
				foreach (var solutionProject in solution.Projects)
				{
					var compilation = solutionProject.GetCompilationAsync().Result;
					if (compilation != null)
					{
						assemblySymbolList.Add(compilation.Assembly);
					}
				}
				_ALL_ASSEMBLY_SYMBOLS = new List<IAssemblySymbol>(assemblySymbolList);
			}

			return _ALL_ASSEMBLY_SYMBOLS;
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
		/// Returns a generic C# <see cref="ITypeSymbol"/> for <see cref="List{T}"/>.
		/// </summary>
		public static ITypeSymbol GetGenericListTypeSymbol()
		{
			return GetGenericCollectionTypeSymbol();
		}

		/// <summary>
		/// Returns a generic C# <see cref="ITypeSymbol"/> for <see cref="List{T}"/>.
		/// </summary>
		public static ITypeSymbol GetGenericCollectionTypeSymbol()
		{
			var creatureTypeSymbol = GetAllFixtureTypeSymbols().First(x => x.Name == "CreatureType");
			var typeSymbol = (ITypeSymbol)creatureTypeSymbol.GetAttributes(nameof(FactoryKeyEnumForAttribute))
				.First(x => x.ConstructorArguments[0].Value.ToString() == "System.Collections.Generic.List<UnityEngine.GameObject>")
				.ConstructorArguments[0]
				.Value;

			return typeSymbol;
		}

		/// <summary>
		/// Returns a C# <see cref="ITypeSymbol"/> that for a <see cref="string"/>.
		/// </summary>
		public static ITypeSymbol GetStringTypeSymbol()
		{
			var creatureTypeSymbol = GetAllFixtureTypeSymbols().First(x => x.Name == "CreatureType");
			var typeSymbol = (ITypeSymbol)creatureTypeSymbol.GetAttributes(nameof(FactoryKeyEnumForAttribute))
				.First(x => x.ConstructorArguments[0].Value.ToString() == "string")
				.ConstructorArguments[0]
				.Value;

			return typeSymbol;
		}

		/// <summary>
		/// Returns a C# <see cref="ITypeSymbol"/> that implements <see cref="ICloneable"/> and has a copy constructor.
		/// </summary>
		public static ITypeSymbol GetCloneableClassTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().Single(x => x.Name == "CloneableClass");
		}

		/// <summary>
		/// Returns a C# <see cref="ITypeSymbol"/> that does not have a default constructor.
		/// </summary>
		public static ITypeSymbol GetClassTypeSymbolWithNoDefaultConstructor()
		{
			return GetAllFixtureTypeSymbols().Single(x => x.Name == "NoDefaultConstructorClass");
		}

		/// <summary>
		/// Returns a generic C# <see cref="ITypeSymbol"/> for <see cref="Dictionary{TKey, TValue}"/>.
		/// </summary>
		public static ITypeSymbol GetGenericDictionaryTypeSymbol()
		{
			var creatureTypeSymbol = GetAllFixtureTypeSymbols().First(x => x.Name == "CreatureType");
			var typeSymbol = (ITypeSymbol)creatureTypeSymbol.GetAttributes(nameof(FactoryKeyEnumForAttribute))
				.First(x => x.ConstructorArguments[0].Value.ToString() == "System.Collections.Generic.Dictionary<int, UnityEngine.GameObject>")
				.ConstructorArguments[0]
				.Value;

			return typeSymbol;
		}

		/// <summary>
		/// Returns a generic C# <see cref="ITypeSymbol"/> for <see cref="List{T}"/>.
		/// </summary>
		public static ITypeSymbol GetClosedGenericTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "ClosedGenericBehaviour");
		}

		/// <summary>
		/// Returns a C# <see cref="ITypeSymbol"/> for an enum type.
		/// </summary>
		public static ITypeSymbol GetEnumTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "CreatureType");
		}

		/// <summary>
		/// Returns a C# <see cref="ITypeSymbol"/> that is contained in the global namespace.
		/// </summary>
		public static ITypeSymbol GetGlobalNamespaceTypeSymbol()
		{
			var globalTypeSymbol = GetAllFixtureTypeSymbols().First(x => x.Name == "CSharpAssemblyClass");
			return globalTypeSymbol;
		}

		/// <summary>
		/// Returns an array C# <see cref="ITypeSymbol"/>.
		/// </summary>
		public static ITypeSymbol GetArrayTypeSymbol()
		{
			var creatureTypeSymbol = GetAllFixtureTypeSymbols().First(x => x.Name == "CreatureType");
			var arrayTypeSymbol = (ITypeSymbol)creatureTypeSymbol.GetAttributes(nameof(FactoryKeyEnumForAttribute))
				.First(x => x.ConstructorArguments[0].Value.ToString() == "UnityEngine.GameObject[]")
				.ConstructorArguments[0]
				.Value;

			return arrayTypeSymbol;
		}

		/// <summary>
		/// Returns an array C# <see cref="ITypeSymbol"/>.
		/// </summary>
		public static ITypeSymbol GetMultiDimArrayTypeSymbol()
		{
			var creatureTypeSymbol = GetAllFixtureTypeSymbols().First(x => x.Name == "CreatureType");
			var arrayTypeSymbol = (ITypeSymbol)creatureTypeSymbol.GetAttributes(nameof(FactoryKeyEnumForAttribute))
				.First(x =>
				{
					var strValue = x.ConstructorArguments[0].Value.ToString();
					return strValue == "UnityEngine.GameObject[*,*]";
				})
				.ConstructorArguments[0]
				.Value;

			return arrayTypeSymbol;
		}

		/// <summary>
		/// Returns a <see cref="ITypeSymbol"/> from the "Assembly-Csharp" assembly with many member type examples.
		/// </summary>
		public static ITypeSymbol GetClassMembersTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "ClassMembersExample");
		}

		/// <summary>
		/// Returns a <see cref="ITypeSymbol"/> from the "Assembly-Csharp" assembly for a delegate type.
		/// </summary>
		public static ITypeSymbol GetDelegateTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "DelegateType");
		}

		/// <summary>
		/// Returns a <see cref="ITypeSymbol"/> from the "Assembly-Csharp" assembly for a nested class type.
		/// </summary>
		public static ITypeSymbol GetNestedTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "NestedClassType");
		}

		/// <summary>
		/// Returns a <see cref="ITypeSymbol"/> from the "Assembly-Csharp" assembly for a Attribute-derived class.
		/// </summary>
		public static ITypeSymbol GetAttributeTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "ExampleAttribute");
		}

		/// <summary>
		/// Returns a <see cref="ITypeSymbol"/> from the "Editor" assembly.
		/// </summary>
		public static ITypeSymbol GetEditorTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "EditorClass");
		}

		/// <summary>
		/// Returns a <see cref="ITypeSymbol"/> from the "Runtime" assembly.
		/// </summary>
		public static ITypeSymbol GetRuntimeTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "RuntimeClass");
		}

		/// <summary>
		/// Returns a <see cref="ITypeSymbol"/> from the "Runtime" assembly.
		/// </summary>
		public static ITypeSymbol GetDerivedAttributeClassTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "FooDerivedClassExample");
		}

		/// <summary>
		/// Returns a <see cref="ITypeSymbol"/> from the "Assembly-Csharp" assembly for a Unity MonoBehaviour derived
		/// type.
		/// </summary>
		public static ITypeSymbol GetUnityMonoBehaviourTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "ExampleMonoBehaviour");
		}

		/// <summary>
		/// Returns a <see cref="ITypeSymbol"/> from the "Assembly-Csharp" assembly for a Unity ScriptableObject derived
		/// type.
		/// </summary>
		public static ITypeSymbol GetUnityScriptableObjectTypeSymbol()
		{
			return GetAllFixtureTypeSymbols().First(x => x.Name == "ExampleScriptableObject");
		}

		/// <summary>
		/// Returns a <see cref="ITypeSymbol"/> from the "Assembly-Csharp" assembly for a Unity ScriptableObject derived
		/// type.
		/// </summary>
		public static ITypeSymbol GetUnityGameObjectTypeSymbol()
		{
			var creatureTypeSymbol = GetAllFixtureTypeSymbols().First(x => x.Name == "CreatureType");
			var typeSymbol = (ITypeSymbol)creatureTypeSymbol.GetAttributes(nameof(FactoryKeyEnumForAttribute))
				.First(x => x.ConstructorArguments[0].Value.ToString() == "UnityEngine.GameObject")
				.ConstructorArguments[0]
				.Value;

			return typeSymbol;
		}

		/// <summary>
		/// Returns a <see cref="ISymbol"/> from the "Assembly-Csharp" assembly for a multiple generic class.
		/// </summary>
		public static ISymbol GetCompilableStringGenericSymbol()
		{
			return GetAllFixtureTypeSymbols()
				.First(x => x.Name == "ClassMembersExample")
				.GetAllMembers()
				.First(x => x.Name == "multipleGenerics");
		}
	}
}
