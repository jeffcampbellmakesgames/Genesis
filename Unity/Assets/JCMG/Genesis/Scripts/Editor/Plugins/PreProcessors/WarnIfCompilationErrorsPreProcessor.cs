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
using UnityEditor;

namespace JCMG.Genesis.Editor.Plugins
{
	internal sealed class WarnIfCompilationErrorsPreProcessor : IPreProcessor
	{
		public string Name => NAME;

		public int Priority => -5;

		public bool RunInDryMode => true;

		private const string NAME = "Warn If Compilation Errors";

		public void PreProcess()
		{
			string str = null;
			if (EditorApplication.isCompiling)
			{
				str = "Cannot generate because Unity is still compiling. Please wait...";
			}

			var assembly = typeof(UnityEditor.Editor).Assembly;
			var type = assembly.GetType("UnityEditorInternal.LogEntries") ?? assembly.GetType("UnityEditor.LogEntries");
			type.GetMethod("Clear").Invoke(new object(), null);
			if ((int)type.GetMethod("GetCount").Invoke(new object(), null) != 0)
			{
				str = "There are compilation errors! Please fix all errors first.";
			}

			if (str != null)
			{
				throw new Exception(str + "\n\nYou can disable this warning by removing '" + Name + "' from the Pre Processors.");
			}
		}
	}
}
