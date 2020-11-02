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

using System.IO;
using Genesis.Plugin;
using Genesis.Shared;
using Serilog;

namespace Genesis.Core.Plugin
{
	/// <summary>
	/// An <see cref="IPostProcessor"/> for removing all files from the output folder for code-generated files.
	/// </summary>
	internal sealed class CleanTargetDirectoryPostProcessor : IPostProcessor,
	                                                          IConfigurable
	{
		public string Name => NAME;

		public int Priority => 94;

		public bool RunInDryMode => false;

		private readonly ILogger _logger = Log.ForContext<CleanTargetDirectoryPostProcessor>();
		private readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

		private const string NAME = "Clean Target Directory";

		public void Configure(IGenesisConfig genesisConfig)
		{
			_targetDirectoryConfig.Configure(genesisConfig);
		}

		public CodeGenFile[] PostProcess(CodeGenFile[] files)
		{
			CleanDirectory();

			return files;
		}

		private void CleanDirectory()
		{
			if (Directory.Exists(_targetDirectoryConfig.TargetDirectory))
			{
				foreach (var file in new DirectoryInfo(_targetDirectoryConfig.TargetDirectory).GetFiles(
					"*.cs",
					SearchOption.AllDirectories))
				{
					try
					{
						File.Delete(file.FullName);
					}
					catch
					{
						_logger.Error("Could not delete file " + file);
					}
				}
			}
			else
			{
				Directory.CreateDirectory(_targetDirectoryConfig.TargetDirectory);
			}
		}
	}
}
