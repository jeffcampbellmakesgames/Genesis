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
			Default = "./plugins")]
		public string PluginPath { get; set; }

		[Option(
			"verbose",
			HelpText = "Sets the logging to be verbose if true, errors only if false.",
			Default = false)]
		public bool IsVerbose { get; set; }

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
