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
		///     Returns the full-type name of this <paramref name="namedTypeSymbol" />.
		/// </summary>
		public static string GetFullTypeName(this INamedTypeSymbol namedTypeSymbol)
		{
			return $"{namedTypeSymbol.ContainingNamespace}.{namedTypeSymbol.Name}";
		}

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
			Debug.Assert(namedTypeSymbol.IsArrayType());

			var arrayTypeSymbol = (IArrayTypeSymbol) namedTypeSymbol;
			return (INamedTypeSymbol) arrayTypeSymbol.ElementType;
		}

		/// <summary>
		///     Returns true if this <paramref name="namedTypeSymbol" /> has <see cref="Attribute" />-derived type
		///     <typeparamref name="T" />.
		/// </summary>
		/// <exception cref="Exception">
		///     <typeparamref name="T" /> must be an attribute, otherwise an assertion
		///     will be thrown.
		/// </exception>
		public static bool HasAttribute<T>(this INamedTypeSymbol namedTypeSymbol)
		{
			Debug.Assert(typeof(T).IsAssignableFrom(typeof(Attribute)));

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
			Debug.Assert(typeof(T).IsAssignableFrom(typeof(Attribute)));

			return namedTypeSymbol.GetAttributes()
				.Where(attr =>
					attr.AttributeClass != null &&
					attr.AttributeClass.Name == nameof(T))
				.Select(attr => attr.ConstructorArguments);
		}

		/// <summary>
		/// </summary>
		public static string ToCompilableString(this INamedTypeSymbol namedTypeSymbol)
		{
			// TODO Implement
			var fullTypeName = namedTypeSymbol.GetFullTypeName();
			if (SerializationTools.TryGetBuiltInTypeToString(fullTypeName, out var str)) return str;

			if (namedTypeSymbol.IsGenericType)
			{
				return fullTypeName.Split('`')[0] +
				       "<" +
				       string.Join(", ",
					       //fullTypeName
					       namedTypeSymbol.TypeArguments
						       .OfType<INamedTypeSymbol>()
						       .Select(argType => argType.ToCompilableString())
						       .ToArray()) +
				       ">";
			}

			if (namedTypeSymbol.IsArrayType())
			{
				var arrayTypeSymbol = (IArrayTypeSymbol) namedTypeSymbol;
				var elementType = namedTypeSymbol.GetArrayElementType();
				return elementType.ToCompilableString() + "[" + new string(',', arrayTypeSymbol.Rank - 1) + "]";
			}

			return namedTypeSymbol.GetFullTypeName();
			//var fullTypeName = namedTypeSymbol.GetFullTypeName();
			//return namedTypeSymbol.IsNested ? fullTypeName.Replace('+', '.') : fullTypeName;
		}
	}
}
