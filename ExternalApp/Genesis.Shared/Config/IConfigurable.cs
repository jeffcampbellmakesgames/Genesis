namespace Genesis.Shared
{
	/// <summary>
	/// Indicates that an object can be configured based on project settings in <see cref="IGenesisConfig"/>.
	/// </summary>
	public interface IConfigurable
	{
		/// <summary>
		/// Configures preferences
		/// </summary>
		/// <param name="genesisConfig"></param>
		void Configure(IGenesisConfig genesisConfig);
	}
}
