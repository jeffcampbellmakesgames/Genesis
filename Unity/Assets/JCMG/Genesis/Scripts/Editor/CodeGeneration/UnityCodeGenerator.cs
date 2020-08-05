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
using UnityEditor;

namespace JCMG.Genesis.Editor
{
	public static class UnityCodeGenerator
	{
		private static readonly ILogger LOGGER;

		static UnityCodeGenerator()
		{
			LOGGER = Log.GetLogger(typeof(UnityCodeGenerator).FullName);
		}

		/// <summary>
		/// Executes a code generation run using all <see cref="GenesisSettings"/> assets found in the project.
		/// </summary>
		public static void GenerateAll()
		{
			var allSettings = GenesisSettings.GetAllSettings();

			Generate(allSettings);
		}

		/// <summary>
		/// Executes a code generation run using the single passed <paramref name="settings"/> asset.
		/// </summary>
		/// <param name="settings"></param>
		public static void GenerateSingle(GenesisSettings settings)
		{
			Generate(new []{settings});
		}

		/// <summary>
		/// Executes a code generation run using the passed <paramref name="settingsData"/> assets.
		/// </summary>
		/// <param name="settingsData"></param>
		public static void GenerateMultiple(GenesisSettings[] settingsData)
		{
			Generate(settingsData);
		}

		/// <summary>
		/// Generates code from all passed <see cref="GenesisSettings"/> assets in <paramref name="settingsData"/>
		/// </summary>
		/// <param name="settingsData"></param>
		private static void Generate(GenesisSettings[] settingsData)
		{
			LOGGER.Info("Generating...");
			var didSucceed = true;
			try
			{
				EditorApplication.LockReloadAssemblies();
				AssetDatabase.StartAssetEditing();

				for (var i = 0; i < settingsData.Length; i++)
				{
					var settings = settingsData[i];
					var codeGenerator = CodeGeneratorTools.CodeGeneratorFromPreferences(settings);
					var progressOffset = 0.0f;
					codeGenerator.OnProgress += (title, info, progress) =>
					{
						if (!EditorUtility.DisplayCancelableProgressBar(title, info, progressOffset + progress / 2f))
						{
							return;
						}

						codeGenerator.Cancel();
					};

					try
					{
						if(GenesisPreferences.ExecuteDryRun)
						{
							codeGenerator.DryRun();
						}

						progressOffset = 0.5f;
						codeGenerator.Generate();
					}
					catch (Exception ex)
					{
						LOGGER.Error(ex, string.Format(EditorConstants.CODE_GENERATION_UPDATE_ERROR_FORMAT, settings.name));

						didSucceed = false;
						EditorUtility.DisplayDialog("Code Generation Error", ex.Message, "Ok");
					}

					EditorUtility.ClearProgressBar();
				}
			}
			catch (Exception ex)
			{
				LOGGER.Error(ex, EditorConstants.CODE_GENERATION_UPDATE_ERROR);
				didSucceed = false;
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
				EditorApplication.UnlockReloadAssemblies();
				EditorUtility.ClearProgressBar();
			}

			if (didSucceed)
			{
				LOGGER.Info(EditorConstants.CODE_GENERATION_SUCCESS);
			}
			else
			{
				LOGGER.Warn(EditorConstants.CODE_GENERATION_FAILURE);
			}

			AssetDatabase.Refresh();
		}
	}
}
