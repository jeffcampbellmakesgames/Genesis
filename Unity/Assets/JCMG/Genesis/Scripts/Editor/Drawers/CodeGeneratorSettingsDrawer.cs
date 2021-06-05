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
using Genesis.Shared;
using UnityEditor;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// A settings drawer for core general settings for Genesis
	/// </summary>
	internal sealed class CodeGeneratorSettingsDrawer : AbstractSettingsDrawer
	{
		public override string Title => TITLE;

		/// <summary>
		/// The ascending order in which this drawer should be shown with regards to other <see cref="ISettingsDrawer"/>
		/// instances
		/// </summary>
		public override int Order => 0;

		private const string TITLE = "General Code Generation";
		private readonly CodeGeneratorConfig _codeGeneratorConfig;

		public CodeGeneratorSettingsDrawer()
		{
			_codeGeneratorConfig = new CodeGeneratorConfig();
		}

		public override void Initialize(GenesisSettings settings)
		{
			// Add default code gen preferences.
			_codeGeneratorConfig.Configure(settings);
		}

		private static string FormatTypeName(string typeName)
		{
			var splitTypeName = typeName.Split('.');
			return splitTypeName[splitTypeName.Length - 1];
		}

		protected override void DrawContentBody(GenesisSettings settings)
		{
			_codeGeneratorConfig.Configure(settings);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Auto Import Plugins");
			if (EditorGUILayoutTools.MiniButton("Auto Import"))
			{
				AutoImport(settings);
			}

			EditorGUILayout.EndHorizontal();

			_codeGeneratorConfig.EnabledPreProcessors = DrawMaskField(
				"Pre Processors",
				_codeGeneratorConfig.AllPreProcessors,
				_codeGeneratorConfig.AllPreProcessors.Select(FormatTypeName).ToArray(),
				_codeGeneratorConfig.EnabledPreProcessors);

			_codeGeneratorConfig.EnabledDataProviders = DrawMaskField(
				"Data Providers",
				_codeGeneratorConfig.AllDataProviders,
				_codeGeneratorConfig.AllDataProviders.Select(FormatTypeName).ToArray(),
				_codeGeneratorConfig.EnabledDataProviders);

			_codeGeneratorConfig.EnabledCodeGenerators = DrawMaskField(
				"Code Generators",
				_codeGeneratorConfig.AllCodeGenerators,
				_codeGeneratorConfig.AllCodeGenerators.Select(FormatTypeName).ToArray(),
				_codeGeneratorConfig.EnabledCodeGenerators);

			_codeGeneratorConfig.EnabledPostProcessors = DrawMaskField(
				"Post Processors",
				_codeGeneratorConfig.AllPostProcessors,
				_codeGeneratorConfig.AllPostProcessors.Select(FormatTypeName).ToArray(),
				_codeGeneratorConfig.EnabledPostProcessors);
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

			//var searchPaths = CodeGeneratorTools.BuildSearchPaths(
			//	_codeGeneratorConfig.SearchPaths,
			//	new []
			//	{
			//		"./Assets",
			//		"./Library/ScriptAssemblies"
			//	});

			//CodeGeneratorTools.AutoImport(_codeGeneratorConfig, searchPaths);

			GenesisCLIRunner.RunConfigurationImport(settings);

			EditorUtility.SetDirty(settings);

			//Initialize(settings);

			//_codeGeneratorConfig.PreProcessors = _availablePreProcessorTypes;
			//_codeGeneratorConfig.DataProviders = _availableDataProviderTypes;
			//_codeGeneratorConfig.CodeGenerators = _availableGeneratorTypes;
			//_codeGeneratorConfig.PostProcessors = _availablePostProcessorTypes;
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
