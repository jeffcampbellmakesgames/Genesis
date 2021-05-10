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

using System.Collections.Generic;
using System.Threading.Tasks;
using LazyCache;

namespace Genesis.Plugin
{
	/// <summary>
	/// Represents an in-memory, thread-safe cache.
	/// </summary>
	internal sealed class MemoryCache : IMemoryCache
	{
		/// <summary>
		/// Returns the number of items in the cache.
		/// </summary>
		public int Count
		{
			get
			{
				lock (_keys)
				{
					return _keys.Count;
				}
			}
		}

		private readonly CachingService _cacheService;
		private readonly HashSet<string> _keys;

		public MemoryCache()
		{
			_cacheService = new CachingService();
			_keys = new HashSet<string>();
		}

		public MemoryCache(CachingService cacheService)
		{
			_cacheService = cacheService;
			_keys = new HashSet<string>();
		}

		/// <summary>
		/// Adds <typeparamref name="T"/> <paramref name="item"/> into the cache with <paramref name="key"/>.
		/// </summary>
		public void Add<T>(string key, T item)
		{
			_cacheService.Add(key, item);

			lock (_keys)
			{
				if (!_keys.Contains(key))
				{
					_keys.Add(key);
				}
			}
		}

		/// <summary>
		/// Returns <typeparamref name="T"/> item with <paramref name="key"/> if present, otherwise returns null.
		/// </summary>
		public T Get<T>(string key)
		{
			return _cacheService.Get<T>(key);
		}

		/// <summary>
		/// Returns over time <typeparamref name="T"/> item with <paramref name="key"/> if present, otherwise returns
		/// null.
		/// </summary>
		public Task<T> GetAsync<T>(string key)
		{
			return _cacheService.GetAsync<T>(key);
		}

		/// <summary>
		/// Returns true if an item of type <typeparamref name="T"/> is present with <paramref name="key"/>. If true,
		/// <paramref name="item"/> will be initialized with it's value.
		/// </summary>
		public bool TryGet<T>(string key, out T item)
			where T : class
		{
			item = null;
			lock (_keys)
			{
				if (!_keys.Contains(key))
				{
					return false;
				}
			}

			item =  _cacheService.Get<T>(key);
			return item != null;
		}

		/// <summary>
		/// Returns true if an item of type <typeparamref name="T"/> is present with <paramref name="key"/>.
		/// </summary>
		public bool Has<T>(string key)
		{
			lock (_keys)
			{
				if (!_keys.Contains(key))
				{
					return false;
				}
			}

			if (!typeof(T).IsValueType)
			{
				#pragma warning disable RECS0017 // Possible compare of value type with 'null'
				return _cacheService.Get<T>(key) != null;
				#pragma warning restore RECS0017 // Possible compare of value type with 'null'
			}

			return true;
		}

		/// <summary>
		/// Removes item with <paramref name="key"/> if present.
		/// </summary>
		public void Remove(string key)
		{
			_cacheService.Remove(key);

			lock (_keys)
			{
				_keys.Remove(key);
			}
		}

		/// <summary>
		/// Clears all items from the cache.
		/// </summary>
		public void Clear()
		{
			lock (_keys)
			{
				foreach (var key in _keys)
				{
					_cacheService.Remove(key);
				}

				_keys.Clear();
			}
		}
	}
}
