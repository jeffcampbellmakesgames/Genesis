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
using System.Linq;

namespace JCMG.Genesis.Editor.Plugins
{
	internal sealed class ScriptableFactoryDataProvider : IDataProvider,
														  IConfigurable
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

		private AssembliesConfig _assembliesConfig;

		private const string NAME = "Scriptable Factory Data";

		/// <summary>
		/// Creates zero or more <see cref="CodeGeneratorData"/> derived instances for code generation to execute upon.
		/// </summary>
		/// <returns></returns>
		public CodeGeneratorData[] GetData()
		{
			// If we are only searching specific assemblies use that whitelist, otherwise get all loaded assemblies.
			var assemblies = _assembliesConfig.DoUseWhitelistOfAssemblies
				? ReflectionTools.GetAvailableAssemblies(_assembliesConfig.WhiteListedAssemblies)
				: ReflectionTools.GetAvailableAssemblies();

			var types = assemblies
				.SelectMany(x => x.GetTypes()).ToArray();

			var codeGenData = new List<CodeGeneratorData>();
			codeGenData.AddRange(GetFactoryCodeGeneratorData(types));
			codeGenData.AddRange(GetFactoryEnumCodeGeneratorData(types));

			return codeGenData.ToArray();
		}

		private IEnumerable<CodeGeneratorData> GetFactoryCodeGeneratorData(IEnumerable<Type> types)
		{
			return types
				.Where(
					x => x.GetCustomAttributes(typeof(FactoryKeyForAttribute), false).Length > 0)
				.SelectMany(
					y =>
					{
						var attrData = (FactoryKeyForAttribute[])y.GetCustomAttributes(
							typeof(FactoryKeyForAttribute),
							false);

						return attrData.Select(
							z =>
							{
								var data = new FactoryKeyData(y, z.ValueType);
								return data;
							});
					});
		}

		private IEnumerable<CodeGeneratorData> GetFactoryEnumCodeGeneratorData(IEnumerable<Type> types)
		{
			return types
				.Where(
					x => x.IsEnum &&
					     x.GetCustomAttributes(typeof(FactoryKeyEnumForAttribute), false).Length > 0)
				.SelectMany(
					y =>
					{
						var attrData = (FactoryKeyEnumForAttribute[])y.GetCustomAttributes(
							typeof(FactoryKeyEnumForAttribute),
							false);

						return attrData.Select(
							z =>
							{
								var data = new FactoryKeyEnumData(y, z.ValueType);
								return data;
							});
					});
		}

		/// <summary>
		/// Configures preferences
		/// </summary>
		/// <param name="settings"></param>
		public void Configure(GenesisSettings settings)
		{
			_assembliesConfig = settings.CreateAndConfigure<AssembliesConfig>();
		}
	}
}
