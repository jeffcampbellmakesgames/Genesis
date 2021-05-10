using System;
using System.Collections.Generic;
using System.Linq;

namespace Genesis.Plugin
{
	/// <summary>
	///     Helper methods for <see cref="IEnumerable{T}" />
	/// </summary>
	public static class EnumerableExtensions
	{
		private static readonly Func<object, bool> NOT_NULL_CHECK = x => x != null;

		/// <summary>
		/// Returns an <see cref="IEnumerable{T}"/> of all non-null elements.
		/// </summary>
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
			where T : class
		{
			return source == null ?
				EmptyTools.EmptyEnumerable<T>() :
				source.Where((Func<T, bool>) NOT_NULL_CHECK);
		}
	}
}
