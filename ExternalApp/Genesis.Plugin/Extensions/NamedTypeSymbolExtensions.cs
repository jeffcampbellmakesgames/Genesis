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
