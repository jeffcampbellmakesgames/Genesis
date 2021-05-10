using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	/// Helper methods for <see cref="IAssemblySymbol" />.
	/// </summary>
	public static class IAssemblySymbolExtensions
	{
		private const string ATTRIBUTE_SUFFIX = "Attribute";

		/// <summary>
		/// Returns true if any of the <paramref name="assemblies"/> contains a namespace with
		/// <paramref name="namespaceName"/>, otherwise false.
		/// </summary>
		public static bool ContainsNamespaceName(this IEnumerable<IAssemblySymbol> assemblies, string namespaceName)
		{
			foreach (var a in assemblies)
			{
				if (a.ContainsNamespaceName(namespaceName))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if <paramref name="assemblySymbol"/> contains a namespace with
		/// <paramref name="namespaceName"/>, otherwise false.
		/// </summary>
		public static bool ContainsNamespaceName(this IAssemblySymbol assemblySymbol, string namespaceName)
		{
			return assemblySymbol.NamespaceNames.Contains(namespaceName);
		}

		/// <summary>
		/// Returns true if these <see cref="IAssemblySymbol"/>s contains a type whose name matches
		/// <paramref name="typeName"/>, otherwise false.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="tryWithAttributeSuffix">If <paramref name="tryWithAttributeSuffix"/> is true,
		/// <paramref name="typeName"/> and <paramref name="typeName"/> + "Attribute" will be searched for.</param>
		/// <param name="assemblies"></param>
		public static bool ContainsTypeName(
			this IEnumerable<IAssemblySymbol> assemblies,
			string typeName,
			bool tryWithAttributeSuffix = false)
		{
			if (!tryWithAttributeSuffix)
			{
				foreach (var a in assemblies)
				{
					if (a.ContainsTypeName(typeName))
					{
						return true;
					}
				}
			}
			else
			{
				foreach (var a in assemblies)
				{
					if (a.ContainsTypeName(typeName, true))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if this <see cref="IAssemblySymbol"/> contains a type whose name matches
		/// <paramref name="typeName"/>, otherwise false.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="tryWithAttributeSuffix">If <paramref name="tryWithAttributeSuffix"/> is true,
		/// <paramref name="typeName"/> and <paramref name="typeName"/> + "Attribute" will be searched for.</param>
		/// <param name="assemblySymbol"></param>
		public static bool ContainsTypeName(
			this IAssemblySymbol assemblySymbol,
			string typeName,
			bool tryWithAttributeSuffix = false)
		{
			if (!tryWithAttributeSuffix)
			{
				return assemblySymbol.TypeNames.Contains(typeName);
			}
			else
			{
				var attributeName = typeName + ATTRIBUTE_SUFFIX;
				var typeNames = assemblySymbol.TypeNames;
				if (typeNames.Contains(typeName) || typeNames.Contains(attributeName))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if this <see cref="IAssemblySymbol"/> is the same or has friend access to
		/// <paramref name="toAssembly"/>.
		/// </summary>
		public static bool IsSameAssemblyOrHasFriendAccessTo(this IAssemblySymbol assembly, IAssemblySymbol toAssembly)
		{
			return
				SymbolEqualityComparer.Default.Equals(assembly, toAssembly) ||
				assembly.IsInteractive && toAssembly.IsInteractive ||
				toAssembly.GivesAccessTo(assembly);
		}
	}
}
