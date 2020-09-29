using System.Collections.Generic;
using System.Linq;

namespace Genesis.Plugin
{
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Merges two dictionaries into a new single instance; any duplicate KVPs are removed.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="dictionaries"></param>
		/// <returns></returns>
		public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
			this Dictionary<TKey, TValue> dictionary,
			params Dictionary<TKey, TValue>[] dictionaries)
		{
			return dictionaries.Aggregate(
				dictionary,
				(current, dict) => current.Union(dict).ToDictionary(kv => kv.Key, kv => kv.Value));
		}
	}
}
