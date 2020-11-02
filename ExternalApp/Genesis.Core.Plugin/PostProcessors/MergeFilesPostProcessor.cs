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
