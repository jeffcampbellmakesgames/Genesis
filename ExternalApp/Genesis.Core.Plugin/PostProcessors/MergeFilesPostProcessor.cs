using System.Collections.Generic;
using System.Linq;
using Genesis.Plugin;

namespace Genesis.Core.Plugin
{
	/// <summary>
	/// A <see cref="IPostProcessor"/> that merges all <see cref="CodeGenFile"/> instances together where the
	/// <see cref="CodeGenFile.FileName"/> is the same.
	/// </summary>
	internal sealed class MergeFilesPostProcessor : IPostProcessor
	{
		public string Name => NAME;

		public int Priority => 99;

		public bool RunInDryMode => true;

		private const string NAME = "Merge files";

		public CodeGenFile[] PostProcess(CodeGenFile[] files)
		{
			var dictionary = new Dictionary<string, CodeGenFile>();
			for (var index = 0; index < files.Length; ++index)
			{
				var file = files[index];
				if (!dictionary.ContainsKey(file.FileName))
				{
					dictionary.Add(file.FileName, file);
				}
				else
				{
					var codeGenFile1 = dictionary[file.FileName];
					codeGenFile1.FileContent = codeGenFile1.FileContent + "\n" + file.FileContent;
					var codeGenFile2 = dictionary[file.FileName];
					codeGenFile2.GeneratorName = codeGenFile2.GeneratorName + ", " + file.GeneratorName;
					files[index] = null;
				}
			}

			return files.Where(file => file != null).ToArray();
		}
	}
}
