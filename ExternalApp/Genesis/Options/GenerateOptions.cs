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
using CommandLine;

namespace Genesis.CLI
{
	/// <summary>
	/// The command-line options for generating code.
	/// </summary>
	[Verb(name:"generate", HelpText = "Generates code based on one or more configuration files.")]
	internal sealed class GenerateOptions
	{
		[Option(
			"project-path",
			HelpText = "The path to the project folder. For Unity, this is the top-level project folder containing " +
			           "the Assets folder.",
			Required = true)]
		public string ProjectPath { get; set; }

		[Option(
			"solution-path",
			HelpText = "The path to the visual studio solution, if any. For Unity, this is the auto-generated " +
			           "solution file .")]
		public string SolutionPath { get; set; }

		[Option(
			longName: "config-file",
			SetName = "file",
			HelpText = "The paths to any config files.",
			Separator = ',')]

		public IEnumerable<string> ConfigFilePaths { get; set; }

		[Option(
			"config-base64",
			SetName = "Base64",
			HelpText = "Any configs encoded as Base64 strings.",
			Separator = ',')]
		public IEnumerable<string> ConfigsAsBase64 { get; set; }

		[Option(
			"plugin-path",
			HelpText = "The path to the plugin folder.",
			Default = "Plugins")]
		public string PluginPath { get; set; }

		[Option(
			"verbose",
			HelpText = "Sets the logging to be verbose if true, errors only if false.",
			Default = false)]
		public bool IsVerbose { get; set; }

		[Option(
			"load-unsafe",
			HelpText = "Forces out-of-date plugins to be loaded",
			Default = false)]
		public bool DoLoadUnsafe { get; set; }

		[Option(
			"dryrun",
			HelpText = "Performs a dry run of code-generation process, but does not output files.",
			Default = false)]
		public bool IsDryRun { get; set; }

		/// <summary>
		/// Returns true if there are file configs in use, otherwise returns false.
		/// </summary>
		public bool HasFileConfigs()
		{
			return ConfigFilePaths.Any();
		}

		/// <summary>
		/// Returns true if there are configs encoded as Base64 string, otherwise returns false.
		/// </summary>
		public bool HasConfigsAsBase64()
		{
			return ConfigsAsBase64.Any();
		}
	}
}
