using CommandLine;

namespace Genesis.CLI
{
	/// <summary>
	/// Command-line options for manipulating or creating configs
	/// </summary>
	[Verb("config", HelpText = "Manipulates or creates a configuration file.")]
	internal sealed class ConfigOptions
	{
		[Option(
			longName:"create",
			SetName = "create",
			HelpText = "Specifies that a config file should be created and populated with all settings from " +
			           "availables plugins.")]
		public bool DoCreate { get; set; }

		[Option(
			longName: "output-path",
			SetName = "create",
			HelpText = "The output file path where this config should be written",
			Default = "./new_config.json")]
		public string CreatePath { get; set; }

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
	}
}
