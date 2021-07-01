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
		/// <param name="canInherit">If true, all base classes of any attributes will be checked and compared for
		/// equality against <paramref name="attributeTypeName"/>.</param>
		public static IEnumerable<AttributeData> GetAttributes(
			this IEnumerable<AttributeData> attributeData,
			string attributeTypeName,
			bool canInherit = true)
		{
			if (canInherit)
			{
				foreach (var attrData in attributeData)
				{
					var attrClasses = attrData.AttributeClass.GetBaseTypesAndThis();
					foreach (var typeSymbol in attrClasses)
					{
						if (typeSymbol.Name == attributeTypeName)
						{
							yield return attrData;
							break;
						}
					}
				}
			}

			foreach (var attrData in attributeData
				.Where(
					attr =>
						attr.AttributeClass != null &&
						attr.AttributeClass.Name == attributeTypeName))
			{
				yield return attrData;
			}
		}

		/// <summary>
		///     Returns true if this <paramref name="attributeData" /> has an <see cref="AttributeData" /> with
		/// <paramref name="attributeTypeName"/>.
		/// </summary>
		/// <param name="canInherit">If true, all base classes of any attributes will be checked and compared for
		/// equality against <paramref name="attributeTypeName"/>.</param>
		public static bool HasAttribute(
			this IEnumerable<AttributeData> attributeData,
			string attributeTypeName,
			bool canInherit = false)
		{
			if (canInherit)
			{
				var hasAttribute = false;
				foreach (var attrData in attributeData)
				{
					var attrClasses = attrData.AttributeClass.GetBaseTypesAndThis();
					foreach (var typeSymbol in attrClasses)
					{
						if (typeSymbol.Name == attributeTypeName)
						{
							hasAttribute = true;
							break;
						}
					}
				}

				return hasAttribute;
			}

			return attributeData.Any(attr =>
				attr.AttributeClass != null &&
				attr.AttributeClass.Name == attributeTypeName);
		}
	}
}
