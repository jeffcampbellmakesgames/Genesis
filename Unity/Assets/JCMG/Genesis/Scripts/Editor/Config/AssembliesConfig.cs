namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// A configuration for enabling user customization of assemblies to search for types via reflection.
	/// </summary>
	public sealed class AssembliesConfig : AbstractConfigurableConfig
	{
		/// <summary>
		/// Returns true if reflection-based logic should only search types in <see cref="WhiteListedAssemblies"/>,
		/// otherwise false.
		/// </summary>
		public bool DoUseWhitelistOfAssemblies
		{
			get
			{
				var value = _settings.GetOrSetValue(DO_USE_WHITE_LIST_KEY, DEFAULT_DO_USE_WHITE_LIST_VALUE).ToLower();

				// If for some reason we can't parse this bool, default to false for white-listing.
				if(!bool.TryParse(value, out var result))
				{
					result = false;
				}

				return result;
			}
			set
			{
				_settings.SetValue(DO_USE_WHITE_LIST_KEY, value.ToString().ToLower());
			}
		}

		/// <summary>
		/// An array of assemblies white-listed for use in reflection; only types from these assemblies will be searched
		/// if <see cref="DoUseWhitelistOfAssemblies"/> is true.
		/// </summary>
		public string[] WhiteListedAssemblies
		{
			get
			{
				return _settings.GetOrSetValue(WHITE_LIST_ASSEMBLIES_KEY, DEFAULT_ASSEMBLIES_VALUE).ArrayFromCSV();
			}
			set
			{
				_settings.SetValue(WHITE_LIST_ASSEMBLIES_KEY, value.ToCSV());
			}
		}

		/// <summary>
		/// Used for UI
		/// </summary>
		internal string RawWhiteListedAssemblies
		{
			get { return _settings.GetOrSetValue(WHITE_LIST_ASSEMBLIES_KEY, DEFAULT_ASSEMBLIES_VALUE); }
			set { _settings.SetValue(WHITE_LIST_ASSEMBLIES_KEY, value); }
		}

		// Keys
		private const string DO_USE_WHITE_LIST_KEY = "Genesis.DoUseWhiteListedAssemblies";
		private const string WHITE_LIST_ASSEMBLIES_KEY = "Genesis.WhiteListedAssemblies";

		// Defaults
		private const string DEFAULT_DO_USE_WHITE_LIST_VALUE = "false";
		private const string DEFAULT_ASSEMBLIES_VALUE = "Assembly-CSharp, Assembly-CSharp-Editor";

		/// <summary>Configures preferences</summary>
		/// <param name="settings"></param>
		public override void Configure(GenesisSettings settings)
		{
			base.Configure(settings);

			settings.SetIfNotPresent(DO_USE_WHITE_LIST_KEY, DEFAULT_DO_USE_WHITE_LIST_VALUE);
			settings.SetIfNotPresent(WHITE_LIST_ASSEMBLIES_KEY, DEFAULT_ASSEMBLIES_VALUE);
		}
	}
}
