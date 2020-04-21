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
using System.Linq;
using System.Text.RegularExpressions;

namespace JCMG.Genesis.Editor.Plugins
{
	internal sealed class UpdateCSharpProjectPostProcessor : IPostProcessor,
	                                                         IConfigurable
	{
		public string Name => NAME;

		public int Priority
		{
			get { return 96; }
		}

		public bool RunInDryMode
		{
			get { return false; }
		}

		private readonly ProjectPathConfig _projectPathConfig;
		private readonly TargetDirectoryConfig _targetDirectoryConfig;

		private const string NAME = "Update CSharp Project";

		public UpdateCSharpProjectPostProcessor()
		{
			_projectPathConfig = new ProjectPathConfig();
			_targetDirectoryConfig = new TargetDirectoryConfig();
		}

		public void Configure(GenesisSettings settings)
		{
			_projectPathConfig.Configure(settings);
			_targetDirectoryConfig.Configure(settings);
		}

		public CodeGenFile[] PostProcess(CodeGenFile[] files)
		{
			File.WriteAllText(
				_projectPathConfig.ProjectPath,
				AddGeneratedEntries(RemoveExistingGeneratedEntries(File.ReadAllText(_projectPathConfig.ProjectPath)), files));
			return files;
		}

		private string RemoveExistingGeneratedEntries(string project)
		{
			var pattern = "\\s*<Compile Include=\"" +
			              _targetDirectoryConfig.TargetDirectory.Replace("/", "\\").Replace("\\", "\\\\") +
			              ".* \\/>";
			project = Regex.Replace(project, pattern, string.Empty);
			project = Regex.Replace(project, "\\s*<ItemGroup>\\s*<\\/ItemGroup>", string.Empty);
			return project;
		}

		private string AddGeneratedEntries(string project, CodeGenFile[] files)
		{
			var entryTemplate = "    <Compile Include=\"" +
			                    _targetDirectoryConfig.TargetDirectory.Replace("/", "\\") +
			                    "\\{0}\" />";
			var replacement = string.Format(
				"</ItemGroup>\n  <ItemGroup>\n{0}\n  </ItemGroup>",
				string.Join(
					"\r\n",
					files.Select(file => string.Format(entryTemplate, file.FileName.Replace("/", "\\"))).ToArray()));
			return new Regex("<\\/ItemGroup>").Replace(project, replacement, 1);
		}
	}
}
