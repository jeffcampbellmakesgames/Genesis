using System.Collections.Generic;
using System.Linq;
using Genesis.Plugin;

namespace Genesis.CLI
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
		}


		public void DryRun(IMemoryCache memoryCache)
		{
			Generate(
				"[Dry Run] ",
				memoryCache,
				_preProcessors.Where(i => i.RunInDryMode).ToArray(),
				_dataProviders.Where(i => i.RunInDryMode).ToArray(),
				_codeGenerators.Where(i => i.RunInDryMode).ToArray(),
				_postProcessors.Where(i => i.RunInDryMode).ToArray());
		}

		public void Generate(IMemoryCache memoryCache)
		{
			Generate(
				string.Empty,
				memoryCache,
				_preProcessors,
				_dataProviders,
				_codeGenerators,
				_postProcessors);
		}

		private void Generate(
			string messagePrefix,
			IMemoryCache memoryCache,
			IPreProcessor[] preProcessors,
			IDataProvider[] dataProviders,
			ICodeGenerator[] codeGenerators,
			IPostProcessor[] postProcessors)
		{
			foreach (var cacheable in preProcessors
				.Concat((IEnumerable<ICodeGenerationPlugin>)dataProviders)
				.Concat(codeGenerators)
				.Concat(postProcessors)
				.OfType<ICacheable>())
			{
				cacheable.SetCache(memoryCache);
			}

			var num1 = preProcessors.Length + dataProviders.Length + codeGenerators.Length + postProcessors.Length;
			var num2 = 0;
			foreach (var preProcessor in preProcessors)
			{
				++num2;

				OnProgress?.Invoke(messagePrefix + "Pre Processing", preProcessor.Name, num2 / (float)num1);

				preProcessor.PreProcess();
			}

			var codeGeneratorDataList = new List<CodeGeneratorData>();
			foreach (var dataProvider in dataProviders)
			{
				++num2;

				OnProgress?.Invoke(messagePrefix + "Creating model", dataProvider.Name, num2 / (float)num1);

				codeGeneratorDataList.AddRange(dataProvider.GetData());
			}

			var codeGenFileList = new List<CodeGenFile>();
			var array = codeGeneratorDataList.ToArray();
			foreach (var codeGenerator in codeGenerators)
			{
				++num2;

				OnProgress?.Invoke(messagePrefix + "Creating files", codeGenerator.Name, num2 / (float)num1);

				codeGenFileList.AddRange(codeGenerator.Generate(array));
			}

			var files = codeGenFileList.ToArray();
			foreach (var postProcessor in postProcessors)
			{
				++num2;

				OnProgress?.Invoke(messagePrefix + "Post Processing", postProcessor.Name, num2 / (float)num1);

				files = postProcessor.PostProcess(files);
			}
		}
	}
}
