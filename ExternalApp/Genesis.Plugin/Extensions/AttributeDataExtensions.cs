using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	/// Helper methods for <see cref="AttributeData"/>.
	/// </summary>
	public static class AttributeDataExtensions
	{
		/// <summary>
		/// Returns all <see cref="AttributeData"/> from this <paramref name="attributeData"/> where the attribute
		/// class's name matches <paramref name="attributeTypeName"/>.
		/// </summary>
		public static IEnumerable<AttributeData> GetAttributes(
			this IEnumerable<AttributeData> attributeData,
			string attributeTypeName)
		{
			return attributeData
				.Where(
					attr =>
						attr.AttributeClass != null &&
						attr.AttributeClass.Name == attributeTypeName);
		}

		/// <summary>
		///     Returns true if this <paramref name="attributeData" /> has an <see cref="AttributeData" /> with
		/// <paramref name="attributeTypeName"/>.
		/// </summary>
		public static bool HasAttribute(
			this IEnumerable<AttributeData> attributeData,
			string attributeTypeName)
		{
			return attributeData.Any(attr =>
				attr.AttributeClass != null &&
				attr.AttributeClass.Name == attributeTypeName);
		}
	}
}
