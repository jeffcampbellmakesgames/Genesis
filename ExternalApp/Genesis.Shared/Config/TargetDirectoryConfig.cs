namespace Genesis.Shared
{
	/// <summary>
	/// A configuration definition for a target directory to write code-generation output to.
	/// </summary>
	public class TargetDirectoryConfig : AbstractConfigurableConfig
	{
		public string TargetDirectory
		{
			get => _genesisConfig.GetOrSetValue(TARGET_DIRECTORY_KEY, ASSETS_FOLDER).ToSafeDirectory();
			set => _genesisConfig.SetValue(TARGET_DIRECTORY_KEY, value.ToSafeDirectory());
		}

		private const string TARGET_DIRECTORY_KEY = "Genesis.TargetDirectory";
		private const string ASSETS_FOLDER = "Assets";

		/// <summary>
		/// Attempts to set defaults for the <paramref name="genesisConfig"/> and initializes any local state.
		/// </summary>
		public override void Configure(IGenesisConfig genesisConfig)
		{
			base.Configure(genesisConfig);

			genesisConfig.SetIfNotPresent(TARGET_DIRECTORY_KEY, ASSETS_FOLDER);
		}
	}
}
