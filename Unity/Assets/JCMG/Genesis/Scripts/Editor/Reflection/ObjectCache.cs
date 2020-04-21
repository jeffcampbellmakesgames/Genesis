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

using System;
using System.Collections.Generic;

namespace JCMG.Genesis.Editor
{
	public class ObjectCache
	{
		private readonly Dictionary<Type, object> _objectPools;

		public ObjectCache()
		{
			_objectPools = new Dictionary<Type, object>();
		}

		public ObjectPool<T> GetObjectPool<T>()
			where T : new()
		{
			var key = typeof(T);
			object obj;
			if (!_objectPools.TryGetValue(key, out obj))
			{
				obj = new ObjectPool<T>(() => new T());
				_objectPools.Add(key, obj);
			}

			return (ObjectPool<T>)obj;
		}

		public T Get<T>()
			where T : new()
		{
			return GetObjectPool<T>().Get();
		}

		public void Push<T>(T obj)
			where T : new()
		{
			GetObjectPool<T>().Push(obj);
		}

		public void RegisterCustomObjectPool<T>(ObjectPool<T> objectPool)
		{
			_objectPools.Add(typeof(T), objectPool);
		}

		public void Reset()
		{
			_objectPools.Clear();
		}
	}
}
