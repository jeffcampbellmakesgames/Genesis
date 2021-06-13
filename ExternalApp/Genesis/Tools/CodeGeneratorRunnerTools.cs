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
