namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Shared constant values for command line parameters
	/// </summary>
	internal static class CommandLineConstants
	{
		// Verbs
		public const string GENERATE_VERB_PARAM = "generate";
		public const string CONFIG_VERB_PARAM = "config";

		// General Options
		public const string VERBOSE_PARAM = "--verbose";

		// Code Generation Options
		public const string DRY_RUN_PARAM = "--dryRun";
		public const string CONFIG_BASE64_PARAM = "--config-base64";
		public const string CONFIG_FILE_PARAM = "--config-file";

		// Config Generation Options
		public const string CONFIG_CREATE_PARAM = "--create";
		public const string OUTPUT_PATH_PARAM = "--output-path";
		public const string PROJECT_PATH_PARAM = "--project-path";
		public const string SOLUTION_PATH_PARAM = "--solution-path";
	}
}
