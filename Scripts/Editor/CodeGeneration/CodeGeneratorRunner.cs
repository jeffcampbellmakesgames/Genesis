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
using System.Linq;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Invoked when code generation process has made some progress.
	/// </summary>
	/// <param name="title"></param>
	/// <param name="info"></param>
	/// <param name="progress"></param>
	public delegate void GeneratorProgress(string title, string info, float progress);

	/// <summary>
	/// Orchestrates code generation via the passed plugin types.
	/// </summary>
	internal sealed class CodeGeneratorRunner
	{
		/// <summary>
		/// Invoked when generator progress has been made.
		/// </summary>
		public event GeneratorProgress OnProgress;

		private readonly IPreProcessor[] _preProcessors;
		private readonly IDataProvider[] _dataProviders;
		private readonly ICodeGenerator[] _codeGenerators;
		private readonly IPostProcessor[] _postProcessors;
		private readonly Dictionary<string, object> _objectCache;

		private bool _cancel;

		public CodeGeneratorRunner(
			IEnumerable<IPreProcessor> preProcessors,
			IEnumerable<IDataProvider> dataProviders,
			IEnumerable<ICodeGenerator> codeGenerators,
			IEnumerable<IPostProcessor> postProcessors)
		{
			_preProcessors = preProcessors.OrderBy(i => i.Priority).ToArray();
			_dataProviders = dataProviders.OrderBy(i => i.Priority).ToArray();
			_codeGenerators = codeGenerators.OrderBy(i => i.Priority).ToArray();
			_postProcessors = postProcessors.OrderBy(i => i.Priority).ToArray();
			_objectCache = new Dictionary<string, object>();
		}


		public CodeGenFile[] DryRun()
		{
			return Generate(
				"[Dry Run] ",
				_preProcessors.Where(i => i.RunInDryMode).ToArray(),
				_dataProviders.Where(i => i.RunInDryMode).ToArray(),
				_codeGenerators.Where(i => i.RunInDryMode).ToArray(),
				_postProcessors.Where(i => i.RunInDryMode).ToArray());
		}

		public CodeGenFile[] Generate()
		{
			var files = Generate(
				string.Empty,
				_preProcessors,
				_dataProviders,
				_codeGenerators,
				_postProcessors);

			return files;
		}

		private CodeGenFile[] Generate(
			string messagePrefix,
			IPreProcessor[] preProcessors,
			IDataProvider[] dataProviders,
			ICodeGenerator[] codeGenerators,
			IPostProcessor[] postProcessors)
		{
			_cancel = false;
			_objectCache.Clear();

			foreach (var cacheable in preProcessors
				.Concat((IEnumerable<ICodeGenerationPlugin>)dataProviders)
				.Concat(codeGenerators)
				.Concat(postProcessors)
				.OfType<ICacheable>())
			{
				cacheable.ObjectCache = _objectCache;
			}

			var num1 = preProcessors.Length + dataProviders.Length + codeGenerators.Length + postProcessors.Length;
			var num2 = 0;
			foreach (var preProcessor in preProcessors)
			{
				if (_cancel)
				{
					return new CodeGenFile[0];
				}

				++num2;

				OnProgress?.Invoke(messagePrefix + "Pre Processing", preProcessor.Name, num2 / (float)num1);

				preProcessor.PreProcess();
			}

			var codeGeneratorDataList = new List<CodeGeneratorData>();
			foreach (var dataProvider in dataProviders)
			{
				if (_cancel)
				{
					return new CodeGenFile[0];
				}

				++num2;

				OnProgress?.Invoke(messagePrefix + "Creating model", dataProvider.Name, num2 / (float)num1);

				codeGeneratorDataList.AddRange(dataProvider.GetData());
			}

			var codeGenFileList = new List<CodeGenFile>();
			var array = codeGeneratorDataList.ToArray();
			foreach (var codeGenerator in codeGenerators)
			{
				if (_cancel)
				{
					return new CodeGenFile[0];
				}

				++num2;

				OnProgress?.Invoke(messagePrefix + "Creating files", codeGenerator.Name, num2 / (float)num1);

				codeGenFileList.AddRange(codeGenerator.Generate(array));
			}

			var files = codeGenFileList.ToArray();
			foreach (var postProcessor in postProcessors)
			{
				if (_cancel)
				{
					return new CodeGenFile[0];
				}

				++num2;

				OnProgress?.Invoke(messagePrefix + "Post Processing", postProcessor.Name, num2 / (float)num1);

				files = postProcessor.PostProcess(files);
			}

			return files;
		}

		public void Cancel()
		{
			_cancel = true;
		}
	}
}
