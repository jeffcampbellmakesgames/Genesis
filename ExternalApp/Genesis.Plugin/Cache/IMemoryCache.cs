/*

MIT License

Copyright (c) Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

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
