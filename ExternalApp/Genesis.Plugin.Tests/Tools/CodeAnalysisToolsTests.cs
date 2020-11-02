using System.Linq;
using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
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
