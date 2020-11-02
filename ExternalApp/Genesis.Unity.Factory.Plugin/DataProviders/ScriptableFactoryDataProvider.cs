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
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Genesis.Unity.Factory.Plugin
{
	/// <summary>
	/// A <see cref="IDataProvider"/> that finds all type symbols decorated with factory attributes.
	/// </summary>
	internal sealed class ScriptableFactoryDataProvider : IDataProvider,
														  ICacheable
	{
		/// <summary>
		/// The name of the plugin.
		/// </summary>
		public string Name => NAME;

		/// <summary>
		/// The priority value this plugin should be given to execute with regards to other plugins,
		/// ordered by ASC value.
		/// </summary>
		public int Priority => 0;

		/// <summary>
		/// Returns true if this plugin should be executed in Dry Run Mode, otherwise false.
		/// </summary>
		public bool RunInDryMode => true;

		private IMemoryCache _memoryCache;

		private const string NAME = "Scriptable Factory Data";

		/// <summary>
		/// Assigns the shared memory cache to this plugin.
		/// </summary>
		public void SetCache(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		/// <summary>
		/// Creates zero or more <see cref="CodeGeneratorData"/> derived instances for code generation to execute upon.
		/// </summary>
		/// <returns></returns>
		public CodeGeneratorData[] GetData()
		{
			var namedTypeSymbols = _memoryCache.GetNamedTypeSymbols();

			var codeGenData = new List<CodeGeneratorData>();
			codeGenData.AddRange(GetFactoryCodeGeneratorData(namedTypeSymbols));
			codeGenData.AddRange(GetFactoryEnumCodeGeneratorData(namedTypeSymbols));

			return codeGenData.ToArray();
		}

		private IEnumerable<CodeGeneratorData> GetFactoryCodeGeneratorData(IReadOnlyList<INamedTypeSymbol> types)
		{
			return types
				.Where(
					x => x.GetAttributes()
						.Any(y => y.AttributeClass != null && y.AttributeClass.Name == nameof(FactoryKeyForAttribute)))
				.SelectMany(
					z =>
					{
						var factoryKeyEnumNamedTypeSymbols = z.GetAttributes()
							.Where(
								attr =>
									attr.AttributeClass != null &&
									attr.AttributeClass.Name == nameof(FactoryKeyForAttribute));

						return factoryKeyEnumNamedTypeSymbols.Select(
							factoryAttr =>
							{
								var value = factoryAttr.ConstructorArguments[0].Value;
								var data = new FactoryKeyData(z, (ITypeSymbol)value);
								return data;
							});
					});
		}

		private IEnumerable<CodeGeneratorData> GetFactoryEnumCodeGeneratorData(IReadOnlyList<INamedTypeSymbol> types)
		{
			return types
				.Where(
					x => x.GetAttributes().Any(y => y.AttributeClass != null
					                                && y.AttributeClass.Name == nameof(FactoryKeyEnumForAttribute)))
				.SelectMany(
					z =>
					{
						var factoryKeyEnumNamedTypeSymbols = z.GetAttributes()
							.Where(
								attr =>
									attr.AttributeClass != null &&
									attr.AttributeClass.Name == nameof(FactoryKeyEnumForAttribute));

						return
							factoryKeyEnumNamedTypeSymbols.Select(
							factoryAttr =>
							{
								var data = new FactoryKeyEnumData(z, (ITypeSymbol)factoryAttr.ConstructorArguments[0].Value);
								return data;
							});
					});
		}
	}
}
