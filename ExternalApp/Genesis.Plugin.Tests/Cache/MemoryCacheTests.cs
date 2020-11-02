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
using LazyCache;
using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
	internal class MemoryCacheTests
	{
		private IMemoryCache _memoryCache;

		private const string KEY = "key";

		[SetUp]
		public void Setup()
		{
			_memoryCache = new MemoryCache(new CachingService());
		}

		[TearDown]
		public void TearDown()
		{
			_memoryCache.Clear();
		}

		[Test]
		public void ValueTypeItemsCanBeAddedToAndRetrievedFromCache()
		{
			var item = 5;

			_memoryCache.Add(KEY, item);

			var cacheItem = _memoryCache.Get<int>(KEY);

			Assert.AreEqual(item, cacheItem);
		}

		[Test]
		public void ReferenceTypeItemsCanBeAddedToAndRetrievedFromCache()
		{
			var item = new List<string>
			{
				"foo", "bar"
			};

			_memoryCache.Add(KEY, item);

			var cacheItem = _memoryCache.Get<List<string>>(KEY);

			Assert.AreEqual(item, cacheItem);
		}

		[Test]
		public void AddingItemsWithSameKeyOverwritesPreviousValue()
		{
			var item = new List<string>
			{
				"foo", "bar"
			};

			_memoryCache.Add(KEY, item);

			var secondItem = new List<string>();

			_memoryCache.Add(KEY, secondItem);

			var cacheItem = _memoryCache.Get<List<string>>(KEY);

			Assert.AreNotEqual(item, cacheItem);
		}

		[Test]
		public void RetrievingValueItemNotInCacheReturnsDefaultValue()
		{
			var nonExistentItem = _memoryCache.Get<int>(KEY);

			Assert.AreEqual(0, nonExistentItem);
		}

		[Test]
		public void RetrievingReferenceItemNotInCacheReturnsNull()
		{
			var nonExistentItem = _memoryCache.Get<List<string>>(KEY);

			Assert.IsNull(nonExistentItem);
		}

		[Test]
		public void ValueItemsCanBeCheckedForInCache()
		{
			Assert.IsFalse(_memoryCache.Has<int>(KEY));

			var item = 5;

			_memoryCache.Add(KEY, item);

			Assert.IsTrue(_memoryCache.Has<int>(KEY));
		}

		[Test]
		public void ReferenceItemsCanBeCheckedForInCache()
		{
			Assert.IsFalse(_memoryCache.Has<List<string>>(KEY));

			var item = new List<string>
			{
				"foo", "bar"
			};

			_memoryCache.Add(KEY, item);

			Assert.IsTrue(_memoryCache.Has<List<string>>(KEY));
			Assert.IsTrue(_memoryCache.TryGet<List<string>>(KEY, out var cachedItem));
			Assert.AreEqual(item, cachedItem);
		}

		[Test]
		public void ValueItemsCanBeRemovedFromCache()
		{
			var item = 5;

			_memoryCache.Add(KEY, item);

			Assert.IsTrue(_memoryCache.Has<int>(KEY));
			Assert.AreEqual(1, _memoryCache.Count);

			_memoryCache.Remove(KEY);

			Assert.IsFalse(_memoryCache.Has<int>(KEY));
			Assert.AreEqual(0, _memoryCache.Count);
		}

		[Test]
		public void ReferenceItemsCanBeRemovedFromCache()
		{
			var item = new List<string>
			{
				"foo", "bar"
			};

			_memoryCache.Add(KEY, item);

			Assert.IsTrue(_memoryCache.Has<List<string>>(KEY));
			Assert.AreEqual(1, _memoryCache.Count);

			_memoryCache.Remove(KEY);

			Assert.IsFalse(_memoryCache.Has<List<string>>(KEY));
			Assert.AreEqual(0, _memoryCache.Count);
		}

		[Test]
		public void ItemsCanBeClearedFromCache()
		{
			Assert.AreEqual(0, _memoryCache.Count);

			var item = 5;

			_memoryCache.Add(KEY, item);

			Assert.AreEqual(1, _memoryCache.Count);

			_memoryCache.Clear();

			Assert.AreEqual(0, _memoryCache.Count);
		}
	}
}
