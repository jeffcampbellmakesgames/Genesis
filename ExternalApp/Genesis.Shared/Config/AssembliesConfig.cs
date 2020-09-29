namespace Genesis.Shared
{
	/// <summary>
	/// A configuration for enabling user customization of assemblies to search for types via reflection.
	/// </summary>
	internal sealed class AssembliesConfig : AbstractConfigurableConfig
	{
		/// <summary>
		/// Returns true if reflection-based logic should only search types in <see cref="WhiteListedAssemblies"/>,
		/// otherwise false.
		/// </summary>
		public bool DoUseWhitelistOfAssemblies
		{
			get
			{
				var value = _genesisConfig.GetOrSetValue(DO_USE_WHITE_LIST_KEY, DEFAULT_DO_USE_WHITE_LIST_VALUE).ToLower();

				// If for some reason we can't parse this bool, default to false for white-listing.
				if(!bool.TryParse(value, out var result))
				{
					result = false;
				}

				return result;
			}
			set => _genesisConfig.SetValue(DO_USE_WHITE_LIST_KEY, value.ToString().ToLower());
		}

		/// <summary>
		/// An array of assemblies white-listed for use in reflection; only types from these assemblies will be searched
		/// if <see cref="DoUseWhitelistOfAssemblies"/> is true.
		/// </summary>
		public string[] WhiteListedAssemblies
		{
			get => _genesisConfig.GetOrSetValue(WHITE_LIST_ASSEMBLIES_KEY, DEFAULT_ASSEMBLIES_VALUE).ArrayFromCSV();
			set => _genesisConfig.SetValue(WHITE_LIST_ASSEMBLIES_KEY, value.ToCSV());
		}

		/// <summary>
		/// Used for UI
		/// </summary>
		internal string RawWhiteListedAssemblies
		{
			get => _genesisConfig.GetOrSetValue(WHITE_LIST_ASSEMBLIES_KEY, DEFAULT_ASSEMBLIES_VALUE);
			set => _genesisConfig.SetValue(WHITE_LIST_ASSEMBLIES_KEY, value);
		}

		// Keys
		private const string DO_USE_WHITE_LIST_KEY = "Genesis.DoUseWhiteListedAssemblies";
		private const string WHITE_LIST_ASSEMBLIES_KEY = "Genesis.WhiteListedAssemblies";

		// Defaults
		private const string DEFAULT_DO_USE_WHITE_LIST_VALUE = "false";
		private const string DEFAULT_ASSEMBLIES_VALUE = "Assembly-CSharp, Assembly-CSharp-Editor";

		/// <summary>
		/// Attempts to set defaults for the <paramref name="genesisConfig"/> and initializes any local state.
		/// </summary>
		public override void Configure(IGenesisConfig genesisConfig)
		{
			base.Configure(genesisConfig);

			genesisConfig.SetIfNotPresent(DO_USE_WHITE_LIST_KEY, DEFAULT_DO_USE_WHITE_LIST_VALUE);
			genesisConfig.SetIfNotPresent(WHITE_LIST_ASSEMBLIES_KEY, DEFAULT_ASSEMBLIES_VALUE);
		}
	}
}
