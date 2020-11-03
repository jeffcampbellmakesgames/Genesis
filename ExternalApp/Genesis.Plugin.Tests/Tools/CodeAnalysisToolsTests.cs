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
using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
	[Category(TestConstants.CATEGORY_CODE_ANALYSIS)]
	internal class CodeAnalysisToolsTests
	{
		[Test]
		public void UnityVSSolutionCanBeParsedForTypeInformation()
		{
			var typeSymbols = TestTools.GetAllFixtureTypeSymbols();

			var runtimeClassNamedTypeSymbol = typeSymbols.FirstOrDefault(x => x.Name == "RuntimeClass");
			var editorClassNamedTypeSymbol = typeSymbols.FirstOrDefault(x => x.Name == "EditorClass");
			var defaultAssemblyNamedTypeSymbol = typeSymbols.FirstOrDefault(x => x.Name == "CSharpAssemblyClass");
			var packageClassNamedTypeSymbol = typeSymbols.FirstOrDefault(x => x.Name == "PackageClass");

			// Verify classes can be successfully found in a variety of Unity scenarios
			Assert.IsNotNull(runtimeClassNamedTypeSymbol);
			Assert.IsNotNull(editorClassNamedTypeSymbol);
			Assert.IsNotNull(defaultAssemblyNamedTypeSymbol);
			Assert.IsNotNull(packageClassNamedTypeSymbol);

			// Verify containing assembly information is correct
			Assert.AreEqual("Runtime", runtimeClassNamedTypeSymbol.ContainingAssembly.Identity.Name);
			Assert.AreEqual("Editor", editorClassNamedTypeSymbol.ContainingAssembly.Identity.Name);
			Assert.AreEqual("Assembly-CSharp", defaultAssemblyNamedTypeSymbol.ContainingAssembly.Identity.Name);
			Assert.AreEqual("Package", packageClassNamedTypeSymbol.ContainingAssembly.Identity.Name);
		}
	}
}
