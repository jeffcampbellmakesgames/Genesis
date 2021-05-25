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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	///     Helper methods for <see cref="INamedTypeSymbol" />.
	/// </summary>
	public static class INamedTypeSymbolExtensions
	{
		/// <summary>
		///     Returns true if this <paramref name="namedTypeSymbol" /> is an array, otherwise false.
		/// </summary>
		/// <param name="namedTypeSymbol"></param>
		/// <returns></returns>
		public static bool IsArrayType(this INamedTypeSymbol namedTypeSymbol)
		{
			return namedTypeSymbol.Kind == SymbolKind.ArrayType;
		}

		/// <summary>
		///     Returns a safe-readable version of a short type name, without generic or array characters.
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
				if (backTickIndex > 0) result = result.Remove(backTickIndex);

				var genericTypeParameters = namedTypeSymbol.TypeParameters.OfType<INamedTypeSymbol>();
				foreach (var genericTypeParameter in genericTypeParameters)
					result += genericTypeParameter.GetHumanReadableName();
			}

			return result;
		}

		/// <summary>
		///     Returns the type name or alias for this <see cref="INamedTypeSymbol" />.
		/// </summary>
		public static string GetTypeNameOrAlias(this INamedTypeSymbol namedTypeSymbol)
		{
			if (namedTypeSymbol.IsValueType &&
			    namedTypeSymbol.ContainingNamespace.ToString() == "System")
				return namedTypeSymbol.GetFullTypeName().GetTypeNameOrAlias();

			return namedTypeSymbol.Name;
		}

		public static INamedTypeSymbol GetElementType(this INamedTypeSymbol namedTypeSymbol)
		{
			if (!namedTypeSymbol.IsArrayType())
			{
				throw new ArgumentException("INamedTypeSymbol must be an array type.");
			}

			var arrayTypeSymbol = (IArrayTypeSymbol) namedTypeSymbol;
			return (INamedTypeSymbol) arrayTypeSymbol.ElementType;
		}

		/// <summary>
		///     Returns true if this <paramref name="namedTypeSymbol" /> has <see cref="Attribute" />-derived type
		///     <typeparamref name="T" />.
		/// </summary>
		/// <exception cref="Exception">
		///     <typeparamref name="T" /> must be an attribute, otherwise an exception
		///     will be thrown.
		/// </exception>
		public static bool HasAttribute<T>(this INamedTypeSymbol namedTypeSymbol)
		{
			if (!typeof(Attribute).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException("T must be assignable to Attribute.");
			}

			return namedTypeSymbol.GetAttributes().Any(attr =>
				attr.AttributeClass != null &&
				attr.AttributeClass.Name == nameof(T));
		}

		/// <summary>
		///     Returns an enumerable of constructor arguments for any <see cref="Attribute" /> instances of type
		///     <typeparamref name="T" />, if any.
		/// </summary>
		/// <exception cref="Exception">
		///     <typeparamref name="T" /> must be an attribute, otherwise an assertion
		///     will be thrown.
		/// </exception>
		public static IEnumerable<ImmutableArray<TypedConstant>> GetAttributeConstructorArguments<T>(
			this INamedTypeSymbol namedTypeSymbol)
		{
			if(!typeof(T).IsAssignableFrom(typeof(Attribute)))
			{
				throw new ArgumentException("T must be assignable to Attribute.");
			}

			return namedTypeSymbol.GetAttributes()
				.Where(attr =>
					attr.AttributeClass != null &&
					attr.AttributeClass.Name == typeof(T).Name)
				.Select(attr => attr.ConstructorArguments);
		}
	}
}
