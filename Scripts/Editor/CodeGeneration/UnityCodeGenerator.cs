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
using System.Linq;
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

		public static void Generate()
		{
			LOGGER.Info("Generating...");
			var didSucceed = true;
			try
			{
				EditorApplication.LockReloadAssemblies();
				AssetDatabase.StartAssetEditing();

				var allSettings = GenesisSettings.GetAllSettings();
				for (var i = 0; i < allSettings.Length; i++)
				{
					var settings = allSettings[i];
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
						didSucceed = false;
						EditorUtility.DisplayDialog("Error", ex.Message, "Ok");
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
