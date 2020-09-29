using System.Threading.Tasks;

namespace Genesis.Plugin
{
	/// <summary>
	/// Represents an in-memory, thread-safe cache.
	/// </summary>
	public interface IMemoryCache
	{
		/// <summary>
		/// Returns the number of items in the cache.
		/// </summary>
		public int Count { get; }

		/// <summary>
		/// Adds <typeparamref name="T"/> <paramref name="item"/> into the cache with <paramref name="key"/>.
		/// </summary>
		void Add<T>(string key, T item);

		/// <summary>
		/// Returns <typeparamref name="T"/> item with <paramref name="key"/> if present, otherwise returns null.
		/// </summary>
		T Get<T>(string key);

		/// <summary>
		/// Returns over time <typeparamref name="T"/> item with <paramref name="key"/> if present, otherwise returns
		/// null.
		/// </summary>
		Task<T> GetAsync<T>(string key);

		/// <summary>
		/// Returns true if an item of type <typeparamref name="T"/> is present with <paramref name="key"/>. If true,
		/// <paramref name="item"/> will be initialized with it's value.
		/// </summary>
		public bool TryGet<T>(string key, out T item)
			where T : class;

		/// <summary>
		/// Returns true if an item of type <typeparamref name="T"/> is present with <paramref name="key"/>.
		/// </summary>
		public bool Has<T>(string key);

		/// <summary>
		/// Removes item with <paramref name="key"/> if present.
		/// </summary>
		void Remove(string key);

		/// <summary>
		/// Clears all items from the cache.
		/// </summary>
		void Clear();
	}
}
