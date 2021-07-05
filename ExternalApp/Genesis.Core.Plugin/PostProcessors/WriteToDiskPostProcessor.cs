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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Genesis.Plugin;
using Genesis.Shared;
using Serilog;
using Serilog.Core;
using OperatingSystem = Genesis.Plugin.OperatingSystem;

namespace Genesis.Core.Plugin
{
	/// <summary>
	/// A <see cref="IPostProcessor"/> that writes all <see cref="CodeGenFile"/> instances to disk.
	/// </summary>
	internal sealed class WriteToDiskPostProcessor : IPostProcessor,
	                                                 IConfigurable
	{
		public string Name => NAME;

		public int Priority => 100;

		public bool RunInDryMode => false;

		private readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

		private const string NAME = "Write Code Gen Files to Disk";

		public void Configure(IGenesisConfig genesisConfig)
		{
			_targetDirectoryConfig.Configure(genesisConfig);
		}

		public CodeGenFile[] PostProcess(CodeGenFile[] files)
		{
			var logger = Log.Logger.ForContext<WriteToDiskPostProcessor>();
			var basePath = _targetDirectoryConfig.TargetDirectory + Path.DirectorySeparatorChar;

			// Get the path-specific slash characters that should be replaced if encountered.
			var replaceSlash = OperatingSystemTools.GetOperatingSystem() switch
			{
				OperatingSystem.macOS => "\\",
				OperatingSystem.Linux => "\\",
				OperatingSystem.Windows => "/",
				_ => throw new ArgumentException(nameof(OSPlatform))
			};
			var pathDirectorySeparatorStr = Path.DirectorySeparatorChar.ToString();

			Parallel.ForEach(files, (codeGenFile, state) =>
			{
				var path = Path.GetFullPath(Path.Combine(basePath, codeGenFile.FileName))
					.Replace(replaceSlash, pathDirectorySeparatorStr);
				var directoryName = Path.GetDirectoryName(path);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}

				using var stream = new FileStream(
					path,
					FileMode.Create,
					FileAccess.Write,
					FileShare.Write,
					4096,
					useAsync: true);
				var bytes = Encoding.UTF8.GetBytes(codeGenFile.FileContent);
				stream.Write(bytes, 0, bytes.Length);
			});

			return files;
		}
	}
}
