namespace Genesis.Shared
{
	/// <summary>
	/// A base class for serializable configs for code generation plugins to persist and expose to users
	/// customized settings.
	/// </summary>
	public abstract class AbstractConfigurableConfig : IConfigurable
	{
		protected IGenesisConfig _genesisConfig;

		/// <summary>
		/// Attempts to set defaults for the <paramref name="genesisConfig"/> and initializes any local state.
		/// </summary>
		public virtual void Configure(IGenesisConfig genesisConfig)
		{
			_genesisConfig = genesisConfig;
		}
	}
}
