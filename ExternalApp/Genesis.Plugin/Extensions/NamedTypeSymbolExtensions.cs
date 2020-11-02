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

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	/// Extension methods for <see cref="INamedTypeSymbol"/>.
	/// </summary>
	public static class NamedTypeSymbolExtensions
	{
		/// <summary>
		/// Returns the name of the assembly that contains this <see cref="INamedTypeSymbol"/>.
		/// </summary>
		public static string GetAssemblyName(this INamedTypeSymbol namedTypeSymbol)
		{
			return namedTypeSymbol.ContainingAssembly.Identity.Name;
		}

		/// <summary>
		/// Returns the full-type name of this <paramref name="namedTypeSymbol"/>.
		/// </summary>
		public static string GetFullTypeName(this INamedTypeSymbol namedTypeSymbol)
		{
			return $"{namedTypeSymbol.ContainingNamespace}.{namedTypeSymbol.Name}";
		}

		/// <summary>
		/// Returns true if this <see cref="namedTypeSymbol"/> is an array, otherwise false.
		/// </summary>
		/// <param name="namedTypeSymbol"></param>
		/// <returns></returns>
		public static bool IsArrayType(this INamedTypeSymbol namedTypeSymbol)
		{
			return namedTypeSymbol.Kind == SymbolKind.ArrayType;
		}

		/// <summary>
		/// Returns a safe-readable version of a short type name, without generic or array characters.
		/// </summary>
		public static string GetHumanReadableName(this INamedTypeSymbol namedTypeSymbol)
		{
			var result = namedTypeSymbol.GetTypeNameOrAlias().UppercaseFirst();
			if (namedTypeSymbol.IsArrayType())
			{
				var elementType = namedTypeSymbol.GetElementType();
				result = string.Format(CodeGenerationConstants.ARRAY_SHORT_NAME, elementType.GetTypeNameOrAlias());
			}
			else if (namedTypeSymbol.IsGenericType)
			{
				var backTickIndex = result.IndexOf(CodeGenerationConstants.BACKTICK_CHAR);
				if (backTickIndex > 0)
				{
					result = result.Remove(backTickIndex);
				}

				var genericTypeParameters = namedTypeSymbol.TypeParameters.OfType<INamedTypeSymbol>();
				foreach (var genericTypeParameter in genericTypeParameters)
				{
					result += genericTypeParameter.GetHumanReadableName();
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the type name or alias for this <see cref="INamedTypeSymbol"/>.
		/// </summary>
		public static string GetTypeNameOrAlias(this INamedTypeSymbol namedTypeSymbol)
		{
			if (namedTypeSymbol.IsValueType &&
			    namedTypeSymbol.ContainingNamespace.ToString() == "System")
			{
				return namedTypeSymbol.GetFullTypeName().GetTypeNameOrAlias();
			}

			return namedTypeSymbol.Name;
		}

		public static INamedTypeSymbol GetElementType(this INamedTypeSymbol namedTypeSymbol)
		{
			Debug.Assert(namedTypeSymbol.IsArrayType());

			var arrayTypeSymbol = (IArrayTypeSymbol)namedTypeSymbol;
			return (INamedTypeSymbol)arrayTypeSymbol.ElementType;
		}
	}
}
