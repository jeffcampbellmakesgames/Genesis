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
using System.Linq;
using UnityEditor;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Represents a set of scripting symbols to apply across all build targets
	/// </summary>
	public sealed class ScriptingDefineSymbols
	{
		public Dictionary<BuildTargetGroup, string> BuildTargetToDefSymbol { get; }

		public ScriptingDefineSymbols()
		{
			BuildTargetToDefSymbol = ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup)))
				.Where(buildTargetGroup =>
				{
					return buildTargetGroup > 0;
				})
				.Where(buildTargetGroup => !IsBuildTargetObsolete(buildTargetGroup))
				.Distinct()
				.ToDictionary(buildTargetGroup => buildTargetGroup, PlayerSettings.GetScriptingDefineSymbolsForGroup);
		}

		public void AddDefineSymbol(string defineSymbol)
		{
			using (var enumerator = BuildTargetToDefSymbol.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var current = enumerator.Current;
					PlayerSettings.SetScriptingDefineSymbolsForGroup(
						current.Key,
						current.Value.Replace(defineSymbol, string.Empty) + "," + defineSymbol);
				}
			}
		}

		public void RemoveDefineSymbol(string defineSymbol)
		{
			using (var enumerator = BuildTargetToDefSymbol.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var current = enumerator.Current;
					PlayerSettings.SetScriptingDefineSymbolsForGroup(
						current.Key,
						current.Value.Replace(defineSymbol, string.Empty));
				}
			}
		}

		private bool IsBuildTargetObsolete(BuildTargetGroup buildTargetGroup)
		{
			return Attribute.IsDefined(
				((object)buildTargetGroup).GetType().GetField(buildTargetGroup.ToString()),
				typeof(ObsoleteAttribute));
		}
	}
}
