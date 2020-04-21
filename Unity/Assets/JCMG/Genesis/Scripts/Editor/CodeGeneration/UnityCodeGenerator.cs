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
			var codeGenerator = CodeGeneratorTools.CodeGeneratorFromPreferences(GenesisSettings.GetOrCreateSettings());
			var progressOffset = 0.0f;
			codeGenerator.OnProgress += (title, info, progress) =>
			{
				if (!EditorUtility.DisplayCancelableProgressBar(title, info, progressOffset + progress / 2f))
				{
					return;
				}

				codeGenerator.Cancel();
			};
			CodeGenFile[] codeGenFileArray1;
			CodeGenFile[] codeGenFileArray2;
			try
			{
				codeGenFileArray1 = GenesisPreferences.ExecuteDryRun
					? codeGenerator.DryRun()
					: new CodeGenFile[0];
				progressOffset = 0.5f;
				codeGenFileArray2 = codeGenerator.Generate();
			}
			catch (Exception ex)
			{
				codeGenFileArray1 = new CodeGenFile[0];
				codeGenFileArray2 = new CodeGenFile[0];
				EditorUtility.DisplayDialog("Error", ex.Message, "Ok");
			}

			EditorUtility.ClearProgressBar();
			LOGGER.Info(
				"Generated " +
				codeGenFileArray2.Select(file => file.FileName).Distinct().Count() +
				" files (" +
				codeGenFileArray1.Select(file => file.FileContent.ToUnixLineEndings())
					.Sum(
						content => content.Split(
								new char[1]
								{
									'\n'
								},
								StringSplitOptions.RemoveEmptyEntries)
							.Length) +
				" sloc, " +
				codeGenFileArray2.Select(file => file.FileContent.ToUnixLineEndings())
					.Sum(content => content.Split('\n').Length) +
				" loc)");

			AssetDatabase.Refresh();
		}
	}
}
