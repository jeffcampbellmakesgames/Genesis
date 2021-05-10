using System;
using System.Collections;
using System.Collections.Generic;

namespace Genesis.Plugin
{
	public static class EmptyTools
	{
		#region Empty Collections

		private static class Empty
		{
			internal static class Array<T>
			{
				#pragma warning disable RECS0108 // Warns about static fields in generic types
				public static readonly T[] INSTANCE = new T[0];
				#pragma warning restore RECS0108 // Warns about static fields in generic types
			}

			internal class Collection<T> : Enumerable<T>, ICollection<T>
			{
				protected static readonly ICollection<T> INSTANCE = new Collection<T>();

				protected Collection()
				{
				}

				public void Add(T item)
				{
					throw new NotSupportedException();
				}

				public void Clear()
				{
				}

				public bool Contains(T item)
				{
					return false;
				}

				public void CopyTo(T[] array, int arrayIndex)
				{
				}

				public int Count => 0;

				public bool IsReadOnly => true;

				public bool Remove(T item)
				{
					throw new NotSupportedException();
				}
			}

			internal class Dictionary<TKey, TValue> : Collection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
			{
				public new static readonly IDictionary<TKey, TValue> INSTANCE = new Dictionary<TKey, TValue>();

				private Dictionary()
				{
				}

				public void Add(TKey key, TValue value)
				{
					throw new NotSupportedException();
				}

				public bool ContainsKey(TKey key)
				{
					return false;
				}

				public ICollection<TKey> Keys => Collection<TKey>.INSTANCE;

				public bool Remove(TKey key)
				{
					throw new NotSupportedException();
				}

				public bool TryGetValue(TKey key, out TValue value)
				{
					value = default(TValue);
					return false;
				}

				public ICollection<TValue> Values => Collection<TValue>.INSTANCE;

				public TValue this[TKey key]
				{
					get => throw new NotSupportedException();

					set => throw new NotSupportedException();
				}

			}

			internal class Enumerable<T> : IEnumerable<T>
			{
				private readonly IEnumerator<T> _enumerator = Enumerator<T>.INSTANCE;

				public IEnumerator<T> GetEnumerator()
				{
					return _enumerator;
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}
			}

			internal class Enumerator : IEnumerator
			{
				public static readonly IEnumerator INSTANCE = new Enumerator();

				protected Enumerator()
				{
				}

				public object Current => throw new InvalidOperationException();

				public bool MoveNext()
				{
					return false;
				}

				public void Reset()
				{
					throw new InvalidOperationException();
				}
			}

			internal class Enumerator<T> : Enumerator, IEnumerator<T>
			{
				public new static readonly IEnumerator<T> INSTANCE = new Enumerator<T>();

				private Enumerator()
				{
				}

				public new T Current => throw new InvalidOperationException();

				public void Dispose()
				{
				}
			}

			internal class List<T> : Collection<T>, IList<T>, IReadOnlyList<T>
			{
				public new static readonly List<T> INSTANCE = new List<T>();

				private List()
				{
				}

				public int IndexOf(T item)
				{
					return -1;
				}

				public void Insert(int index, T item)
				{
					throw new NotSupportedException();
				}

				public void RemoveAt(int index)
				{
					throw new NotSupportedException();
				}

				public T this[int index]
				{
					get => throw new ArgumentOutOfRangeException(nameof(index));

					set => throw new NotSupportedException();
				}
			}

			internal class Set<T> : Collection<T>, ISet<T>
			{
				public new static readonly ISet<T> INSTANCE = new Set<T>();

				private Set()
				{
				}

				public new bool Add(T item)
				{
					throw new NotImplementedException();
				}

				public void ExceptWith(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public void IntersectWith(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool IsProperSubsetOf(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool IsProperSupersetOf(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool IsSubsetOf(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool IsSupersetOf(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool Overlaps(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool SetEquals(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public void SymmetricExceptWith(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public void UnionWith(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public new IEnumerator GetEnumerator()
				{
					return Set<T>.INSTANCE.GetEnumerator();
				}
			}
		}

		#endregion

		public static T[] EmptyArray<T>()
		{
			return Empty.Array<T>.INSTANCE;
		}

		public static IEnumerator<T> EmptyEnumerator<T>()
		{
			return Empty.Enumerator<T>.INSTANCE;
		}

		public static IEnumerable<T> EmptyEnumerable<T>()
		{
			return Empty.List<T>.INSTANCE;
		}

		public static ICollection<T> EmptyCollection<T>()
		{
			return Empty.List<T>.INSTANCE;
		}

		public static IList<T> EmptyList<T>()
		{
			return Empty.List<T>.INSTANCE;
		}

		public static ISet<T> EmptySet<T>()
		{
			return Empty.Set<T>.INSTANCE;
		}

		public static IDictionary<TKey, TValue> EmptyDictionary<TKey, TValue>()
		{
			return Empty.Dictionary<TKey, TValue>.INSTANCE;
		}
	}
}
