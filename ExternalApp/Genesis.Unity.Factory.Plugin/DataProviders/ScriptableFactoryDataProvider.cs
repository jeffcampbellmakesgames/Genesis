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
using Genesis.Shared;
using Microsoft.CodeAnalysis;
using Serilog;

namespace Genesis.Unity.Factory.Plugin
{
	/// <summary>
	/// A <see cref="IDataProvider"/> that finds all type symbols decorated with factory attributes.
	/// </summary>
	internal sealed class ScriptableFactoryDataProvider : IDataProvider,
														  IConfigurable,
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
		private AssembliesConfig _assembliesConfig;
		private readonly ILogger _logger;

		private const string NAME = "Scriptable Factory Data";

		public ScriptableFactoryDataProvider()
		{
			_logger = Log.Logger.ForContext<ScriptableFactoryDataProvider>();
		}

		/// <inheritdoc />
		public void SetCache(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		/// <inheritdoc />
		public void Configure(IGenesisConfig genesisConfig)
		{
			_assembliesConfig = genesisConfig.CreateAndConfigure<AssembliesConfig>();
		}

		/// <inheritdoc />
		public CodeGeneratorData[] GetData()
		{
			var codeGenData = new List<CodeGeneratorData>();
			var filteredTypeSymbols =
				_assembliesConfig.FilterTypeSymbols(_memoryCache.GetNamedTypeSymbols());
			codeGenData.AddRange(GetFactoryCodeGeneratorData(filteredTypeSymbols));
			codeGenData.AddRange(GetFactoryEnumCodeGeneratorData(filteredTypeSymbols));
			codeGenData.AddRange(GetSymbolFactoryCodeGeneratorData(filteredTypeSymbols));
			return codeGenData.ToArray();
		}

		private IEnumerable<CodeGeneratorData> GetFactoryCodeGeneratorData(IReadOnlyList<ICachedNamedTypeSymbol> typeSymbolInfo)
		{
			return typeSymbolInfo
				.Select(x => new
				{
					type = x,
					attributes = x.GetAttributes(nameof(FactoryKeyForAttribute))
				})
				.SelectMany(
					z =>
					{
						return z.attributes.Select(
							factoryAttr =>
							{
								var value = factoryAttr.ConstructorArguments[0].Value;
								var data = new FactoryKeyData(z.type.NamedTypeSymbol, (ITypeSymbol)value);
								return data;
							});
					});
		}

		private IEnumerable<CodeGeneratorData> GetFactoryEnumCodeGeneratorData(IReadOnlyList<ICachedNamedTypeSymbol> types)
		{
			return types
				.Where(x => x.HasAttribute(nameof(FactoryKeyEnumForAttribute)))
				.SelectMany(
					z =>
					{
						var factoryKeyEnumNamedTypeSymbols =
							z.GetAttributes(nameof(FactoryKeyEnumForAttribute));
						return
							factoryKeyEnumNamedTypeSymbols.Select(
							factoryAttr =>
							{
								var data = new FactoryKeyEnumData(
									z.NamedTypeSymbol,
									(ITypeSymbol)factoryAttr.ConstructorArguments[0].Value);
								return data;
							});
					});
		}

		private IEnumerable<CodeGeneratorData> GetSymbolFactoryCodeGeneratorData(IReadOnlyList<ICachedNamedTypeSymbol> types)
		{
			var list = new List<CodeGeneratorData>();
			var decoratedTypes = types
				.Where(x => x.HasAttribute(nameof(GenerateSymbolFactoryAttribute)));
			foreach (var cachedNamedTypeSymbol in decoratedTypes)
			{
				if (cachedNamedTypeSymbol.ImplementsInterface<ISymbolObject>())
				{
					list.Add(new SymbolFactoryData(cachedNamedTypeSymbol.NamedTypeSymbol));
				}
				else
				{
					_logger.Warning($"Cannot create SymbolFactory for {cachedNamedTypeSymbol.FullTypeName} " +
					                $"as it does not implement {nameof(ISymbolObject)}. Skipping....");
				}
			}

			return list;
		}
	}
}
