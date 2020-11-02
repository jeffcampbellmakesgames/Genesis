using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Genesis.Plugin;
using Genesis.Shared;
using Serilog;

namespace Genesis.CLI
{
	/// <summary>
	/// Helper methods for the code generator
	/// </summary>
	internal static class CodeGeneratorRunnerTools
	{
		private static readonly ILogger LOGGER = Log.ForContext(typeof(CodeGeneratorRunnerTools));

		public static CodeGeneratorRunner CodeGeneratorFromPreferences(IGenesisConfig settings)
		{
			var instances = LoadFromPlugins();
			var andConfigure = settings.CreateAndConfigure<CodeGeneratorConfig>();
			var enabledInstancesOf1 = GetEnabledInstancesOf<IPreProcessor>(instances, andConfigure.EnabledPreProcessors).ToArray();
			var enabledInstancesOf2 = GetEnabledInstancesOf<IDataProvider>(instances, andConfigure.EnabledDataProviders).ToArray();
			var enabledInstancesOf3 = GetEnabledInstancesOf<ICodeGenerator>(instances, andConfigure.EnabledCodeGenerators).ToArray();
			var enabledInstancesOf4 = GetEnabledInstancesOf<IPostProcessor>(instances, andConfigure.EnabledPostProcessors).ToArray();

			Configure(enabledInstancesOf1, settings);
			Configure(enabledInstancesOf2, settings);
			Configure(enabledInstancesOf3, settings);
			Configure(enabledInstancesOf4, settings);

			return new CodeGeneratorRunner(
				enabledInstancesOf1,
				enabledInstancesOf2,
				enabledInstancesOf3,
				enabledInstancesOf4);
		}

		private static void Configure(IEnumerable<ICodeGenerationPlugin> plugins, IGenesisConfig settings)
		{
			foreach (var configurable in plugins.OfType<IConfigurable>())
			{
				configurable.Configure(settings);
			}
		}

		public static ICodeGenerationPlugin[] LoadFromPlugins()
		{
			return ReflectionTools.GetAllImplementingTypesOfInterface<ICodeGenerationPlugin>()
				.Select(
					type =>
					{
						try
						{
							return (ICodeGenerationPlugin)Activator.CreateInstance(type);
						}
						catch (TypeLoadException ex)
						{
							LOGGER.Warning(ex.Message);
						}

						return null;
					})
				.Where(instance => instance != null)
				.ToArray();
		}

		public static T[] GetOrderedInstancesOf<T>(ICodeGenerationPlugin[] instances)
			where T : ICodeGenerationPlugin
		{
			return instances.OfType<T>()
				.OrderBy(instance => instance.Priority)
				.ThenBy(instance => instance.GetType().ToCompilableString())
				.ToArray();
		}

		public static IEnumerable<T> GetEnabledInstancesOf<T>(ICodeGenerationPlugin[] instances, string[] typeNames)
			where T : ICodeGenerationPlugin
		{
			return GetOrderedInstancesOf<T>(instances)
				.Where(instance => typeNames.Contains(instance.GetType().ToCompilableString()));
		}

		public static string[] BuildSearchPaths(string[] searchPaths, string[] additionalSearchPaths)
		{
			return searchPaths.Concat(additionalSearchPaths).Where(Directory.Exists).ToArray();
		}

		internal static void AutoImport(CodeGeneratorConfig config, params string[] searchPaths)
		{
			var assemblyPaths = ReflectionTools.GetAllImplementingTypesOfInterface<ICodeGenerationPlugin>()
				.Select(type => type.Assembly)
				.Distinct()
				.Select(assembly => assembly.CodeBase.MakePathRelativeTo(Directory.GetCurrentDirectory()))
				.ToArray();
			var currentFullPaths = new HashSet<string>(config.SearchPaths.Select(Path.GetFullPath));
			var second = assemblyPaths.Select(Path.GetDirectoryName).Where(path => !currentFullPaths.Contains(path));
			config.SearchPaths = config.SearchPaths.Concat(second).Distinct().OrderBy(path => path).ToArray();
			config.Plugins = assemblyPaths.Select(Path.GetFileNameWithoutExtension).Distinct().OrderBy(plugin => plugin).ToArray();
		}

		public static void SetTypes<T>(
			ICodeGenerationPlugin[] instances,
			out string[] availableTypes)
			where T : ICodeGenerationPlugin
		{
			var orderedInstancesOf = GetOrderedInstancesOf<T>(instances);
			availableTypes = orderedInstancesOf.Select(instance => instance.GetType().ToCompilableString()).ToArray();
		}

		public static void SetTypesAndNames<T>(
			ICodeGenerationPlugin[] instances,
			out string[] availableTypes,
			out string[] availableNames)
			where T : ICodeGenerationPlugin
		{
			var orderedInstancesOf = GetOrderedInstancesOf<T>(instances);
			availableTypes = orderedInstancesOf.Select(instance => instance.GetType().ToCompilableString()).ToArray();
			availableNames = orderedInstancesOf.Select(instance => instance.Name).ToArray();
		}
	}
}
