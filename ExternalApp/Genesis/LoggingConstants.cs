namespace Genesis.CLI
{
	/// <summary>
	/// Shared constant fields for logging.
	/// </summary>
	internal static class LoggingConstants
	{
		// Logging Properties
		public const string VERSION_PROPERTY = "Version";

		// Logging Formats
		public static readonly string VERBOSE_LOGGING_TEMPLATE =
			$"[{{Timestamp:HH:mm}}] [{{{VERSION_PROPERTY}}}] [{{Level}}] {{Message}}{{NewLine}}{{Exception}}";

		public static readonly string GENERAL_LOGGING_TEMPLATE =
			$"[{{Timestamp:HH:mm}}] [{{Level}}] {{Message}}{{NewLine}}{{Exception}}";

		// Logging files
		public const string LOG_FILENAME = "./log.txt";
	}
}
