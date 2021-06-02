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

using System.Linq;
using Microsoft.CodeAnalysis;

namespace Genesis.Plugin
{
	/// <summary>
	/// Helper methods for Unity related types.
	/// </summary>
	public static class UnityExtensions
	{
		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is or derives from a Unity Object, otherwise false.
		/// </summary>
		public static bool IsUnityObject(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.InheritsFromOrIs("UnityEngine.Object");
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is or derives from a Unity ScriptableObject, otherwise false.
		/// </summary>
		public static bool IsUnityScriptableObject(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.InheritsFromOrIs("UnityEngine.ScriptableObject");
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is or derives from a Unity MonoBehaviour, otherwise false.
		/// </summary>
		public static bool IsUnityMonoBehaviour(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.InheritsFromOrIs("UnityEngine.MonoBehaviour");
		}

		/// <summary>
		/// Returns true if this <see cref="ITypeSymbol"/> is or derives from a Unity GameObject, otherwise false.
		/// </summary>
		public static bool IsUnityGameObject(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.InheritsFromOrIs("UnityEngine.GameObject");
		}
	}
}
