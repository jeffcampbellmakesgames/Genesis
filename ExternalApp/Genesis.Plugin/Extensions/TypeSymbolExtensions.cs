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
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	/// Helper methods for <see cref="ITypeSymbol"/>.
	/// </summary>
	public static class TypeSymbolExtensions
	{
	/// <summary>
		/// Returns the full-type name of this <paramref name="typeSymbol"/>.
		/// </summary>
		public static string GetFullTypeName(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.ToString();
		}

		/// <summary>
		/// Returns true if this <see cref="typeSymbol"/> is an array, otherwise false.
		/// </summary>
		public static bool IsArrayType(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.Kind == SymbolKind.ArrayType;
		}

		/// <summary>
		/// Returns true if this <see cref="typeSymbol"/> is an array, otherwise false.
		/// </summary>
		public static bool IsGenericType(this ITypeSymbol typeSymbol)
		{
			if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
			{
				return namedTypeSymbol.IsGenericType;
			}

			return typeSymbol.BaseType != null && typeSymbol.BaseType.IsGenericType;
		}

		/// <summary>
		/// Returns a safe-readable version of a short type name, without generic or array characters.
		/// </summary>
		public static string GetHumanReadableName(this ITypeSymbol typeSymbol)
		{
			var result = typeSymbol.Name.UppercaseFirst();
			if (typeSymbol.IsArrayType())
			{
				var elementType = typeSymbol.GetArrayElementType();
				result = string.Format(CodeGenerationConstants.ARRAY_SHORT_NAME, elementType.GetTypeNameOrAlias());
			}
			else if (typeSymbol.IsGenericType())
			{
				var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;
				var leftChevronIndex = result.IndexOf(CodeGenerationConstants.LEFT_CHEVRON_CHAR);
				if (leftChevronIndex > 0)
				{
					result = result.Remove(leftChevronIndex);
				}

				var genericTypeParameters = namedTypeSymbol.TypeArguments;
				foreach (var genericTypeParameter in genericTypeParameters)
				{
					result += genericTypeParameter.GetHumanReadableName();
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the short type name or alias for this <see cref="typeSymbol"/>.
		/// </summary>
		public static string GetTypeNameOrAlias(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.GetFullTypeName().GetTypeNameOrAlias();
		}

		/// <summary>
		/// Returns the array element <see cref="ITypeSymbol"/> for this instance. If not an array type, an exception
		/// will be thrown.
		/// </summary>
		public static INamedTypeSymbol GetArrayElementType(this ITypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsArrayType())
			{
				const string EXCEPTION_MSG = "TypeSymbol [{0}] must be an array type in order to retrieve an " +
				                             "array element type.";

				throw new ArgumentException(
					string.Format(EXCEPTION_MSG, typeSymbol),
					nameof(typeSymbol));
			}

			var arrayTypeSymbol = (IArrayTypeSymbol)typeSymbol;
			return (INamedTypeSymbol)arrayTypeSymbol.ElementType;
		}
	}
}
