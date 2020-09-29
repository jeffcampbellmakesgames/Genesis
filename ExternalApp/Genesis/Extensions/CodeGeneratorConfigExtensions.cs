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
