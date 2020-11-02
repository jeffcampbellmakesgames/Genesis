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
using Genesis.Plugin;
using Genesis.Shared;

namespace Genesis.CLI
{
	/// <summary>
	/// Helper methods for <see cref="CodeGeneratorConfig"/>.
	/// </summary>
	internal static class CodeGeneratorConfigExtensions
	{
		public static void AutoImportPlugins(this CodeGeneratorConfig config, ICodeGenerationPlugin[] plugins)
		{
			var assemblyPaths = plugins
				.Select(x => x.GetType().Assembly)
				.Distinct()
				.Select(assembly => assembly.CodeBase.MakePathRelativeTo(Directory.GetCurrentDirectory()))
				.ToArray();

			var currentFullPaths = new HashSet<string>(config.SearchPaths.Select(Path.GetFullPath));
			var second = assemblyPaths.Select(Path.GetDirectoryName).Where(path => !currentFullPaths.Contains(path));
			config.SearchPaths = config.SearchPaths.Concat(second).Distinct().OrderBy(path => path).ToArray();
			config.Plugins = assemblyPaths.Select(Path.GetFileNameWithoutExtension)
				.Distinct()
				.OrderBy(plugin => plugin)
				.ToArray();

			CodeGeneratorRunnerTools.SetTypes<IPreProcessor>(plugins, out var availablePreProcessorTypes);
			CodeGeneratorRunnerTools.SetTypes<IDataProvider>(plugins, out var availableDataProviderTypes);
			CodeGeneratorRunnerTools.SetTypes<ICodeGenerator>(plugins, out var availableGeneratorTypes);
			CodeGeneratorRunnerTools.SetTypes<IPostProcessor>(plugins, out var availablePostProcessorTypes);

			config.EnabledPreProcessors = availablePreProcessorTypes;
			config.EnabledDataProviders = availableDataProviderTypes;
			config.EnabledCodeGenerators = availableGeneratorTypes;
			config.EnabledPostProcessors = availablePostProcessorTypes;

			config.AllPreProcessors = availablePreProcessorTypes;
			config.AllDataProviders = availableDataProviderTypes;
			config.AllCodeGenerators = availableGeneratorTypes;
			config.AllPostProcessors = availablePostProcessorTypes;
		}
	}
}
