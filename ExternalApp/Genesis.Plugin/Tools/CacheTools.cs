namespace Genesis.Plugin
{
	/// <summary>
	/// Helper methods for caching.
	/// </summary>
	public static class CacheTools
	{
		/// <summary>
		/// Returns a default <see cref="IMemoryCache"/> instance.
		/// </summary>
		public static IMemoryCache CreateDefaultCache()
		{
			return new MemoryCache();
		}
	}
}
