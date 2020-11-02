namespace Genesis.Plugin
{
	/// <summary>
	/// Represent an object that offers some measure of code generation plugin functionality.
	/// </summary>
	public interface ICodeGenerationPlugin
	{
		/// <summary>
		/// The name of the plugin.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The priority value this plugin should be given to execute with regards to other plugins,
		/// ordered by ASC value.
		/// </summary>
		int Priority { get; }

		/// <summary>
		/// Returns true if this plugin should be executed in Dry Run Mode, otherwise false.
		/// </summary>
		bool RunInDryMode { get; }
	}
}
