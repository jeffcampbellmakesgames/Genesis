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
using UnityEditor;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// A preferences drawer for core general settings for Genesis
	/// </summary>
	internal sealed class CodeGeneratorPreferencesDrawer : AbstractPreferencesDrawer
	{
		public override string Title => TITLE;

		private const string TITLE = "General Code Generation";

		private readonly CodeGeneratorConfig _codeGeneratorConfig;

		private string[] _availableDataProviderNames;
		private string[] _availableDataProviderTypes;
		private string[] _availableGeneratorNames;
		private string[] _availableGeneratorTypes;
		private string[] _availablePostProcessorNames;
		private string[] _availablePostProcessorTypes;
		private string[] _availablePreProcessorNames;
		private string[] _availablePreProcessorTypes;
		private ICodeGenerationPlugin[] _instances;

		public CodeGeneratorPreferencesDrawer()
		{
			_codeGeneratorConfig = new CodeGeneratorConfig();
		}

		public override void Initialize(GenesisSettings settings)
		{
			// Add default code gen preferences.
			_codeGeneratorConfig.Configure(settings);

			// Add per plugin interface type preferences.
			_instances = CodeGeneratorTools.LoadFromPlugins();
			SetTypesAndNames<IPreProcessor>(_instances, out _availablePreProcessorTypes, out _availablePreProcessorNames);
			SetTypesAndNames<IDataProvider>(_instances, out _availableDataProviderTypes, out _availableDataProviderNames);
			SetTypesAndNames<ICodeGenerator>(_instances, out _availableGeneratorTypes, out _availableGeneratorNames);
			SetTypesAndNames<IPostProcessor>(_instances, out _availablePostProcessorTypes, out _availablePostProcessorNames);
		}

		protected override void DrawContentBody(GenesisSettings settings)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Auto Import Plugins");
			if (EditorGUILayoutTools.MiniButton("Auto Import"))
			{
				AutoImport(settings);
			}

			EditorGUILayout.EndHorizontal();

			_codeGeneratorConfig.PreProcessors = DrawMaskField(
				"Pre Processors",
				_availablePreProcessorTypes,
				_availablePreProcessorNames,
				_codeGeneratorConfig.PreProcessors);

			_codeGeneratorConfig.DataProviders = DrawMaskField(
				"Data Providers",
				_availableDataProviderTypes,
				_availableDataProviderNames,
				_codeGeneratorConfig.DataProviders);

			_codeGeneratorConfig.CodeGenerators = DrawMaskField(
				"Code Generators",
				_availableGeneratorTypes,
				_availableGeneratorNames,
				_codeGeneratorConfig.CodeGenerators);

			_codeGeneratorConfig.PostProcessors = DrawMaskField(
				"Post Processors",
				_availablePostProcessorTypes,
				_availablePostProcessorNames,
				_codeGeneratorConfig.PostProcessors);
		}

		private void AutoImport(GenesisSettings settings)
		{
			if (!EditorUtility.DisplayDialog(
				"Genesis - Auto Import",
				"Auto Import will automatically find and set all plugins for you. It will search in folders and sub folders specified " +
				"under the key 'Genesis.SearchPaths'.\n\nThis will overwrite your current plugin settings.\n\nDo you want to continue?",
				"Continue and Overwrite",
				"Cancel"))
			{
				return;
			}

			var searchPaths = CodeGeneratorTools.BuildSearchPaths(
				_codeGeneratorConfig.SearchPaths,
				new []
				{
					"./Assets",
					"./Library/ScriptAssemblies"
				});

			CodeGeneratorTools.AutoImport(_codeGeneratorConfig, searchPaths);

			EditorUtility.SetDirty(settings);

			Initialize(settings);

			_codeGeneratorConfig.PreProcessors = _availablePreProcessorTypes;
			_codeGeneratorConfig.DataProviders = _availableDataProviderTypes;
			_codeGeneratorConfig.CodeGenerators = _availableGeneratorTypes;
			_codeGeneratorConfig.PostProcessors = _availablePostProcessorTypes;
		}

		private static void SetTypesAndNames<T>(
			ICodeGenerationPlugin[] instances,
			out string[] availableTypes,
			out string[] availableNames)
			where T : ICodeGenerationPlugin
		{
			var orderedInstancesOf = CodeGeneratorTools.GetOrderedInstancesOf<T>(instances);
			availableTypes = orderedInstancesOf.Select(instance => instance.GetType().ToCompilableString()).ToArray();
			availableNames = orderedInstancesOf.Select(instance => instance.Name).ToArray();
		}

		private static string[] DrawMaskField(
			string title,
			string[] types,
			string[] names,
			string[] input)
		{
			var num1 = 0;
			for (var index = 0; index < types.Length; ++index)
			{
				if (input.Contains(types[index]))
				{
					num1 += 1 << index;
				}
			}

			if (names.Length != 0)
			{
				var num2 = (int)Math.Pow(2.0, types.Length) - 1;
				if (num1 == num2)
				{
					num1 = -1;
				}

				num1 = EditorGUILayout.MaskField(title, num1, names);
			}
			else
			{
				EditorGUILayout.LabelField(title, "No " + title + " available");
			}

			var stringList = new List<string>();
			for (var index = 0; index < types.Length; ++index)
			{
				var num2 = 1 << index;
				if ((num2 & num1) == num2)
				{
					stringList.Add(types[index]);
				}
			}

			stringList.AddRange(input.Where(type => !types.Contains(type)));
			return stringList.ToArray();
		}
	}
}
