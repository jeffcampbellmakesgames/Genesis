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
	internal sealed class WarnIfCompilingPreProcessor : IPreProcessor
	{
		public string Name => NAME;

		public int Priority => -5;

		public bool RunInDryMode => true;

		private const string NAME = "Warn If Compiling";

		public void PreProcess()
		{
			if (EditorApplication.isCompiling)
			{
				throw new Exception("Cannot generate because Unity is still compiling. Please wait...\n\n" +
				                    "You can disable this warning by removing '" + Name + "' from the Pre Processors.");
			}
		}
	}
}
