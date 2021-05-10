using System.Linq;
using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
	[Category(TestConstants.CATEGORY_CODE_ANALYSIS)]
	public static class IAssemblySymbolExtensionTests
	{
		[Test]
		public static void CanFindContainingNamespace()
		{
			var allAssemblySymbols = TestTools.GetAllFixtureAssemblySymbols();
			var csharpAssemblySymbol = allAssemblySymbols.First(x => x.Name == "Assembly-CSharp");

			Assert.True(allAssemblySymbols.ContainsNamespaceName("Fixtures"));
			Assert.True(csharpAssemblySymbol.ContainsNamespaceName("Fixtures"));

			Assert.False(allAssemblySymbols.ContainsNamespaceName("NonExistentNamespace"));
			Assert.False(csharpAssemblySymbol.ContainsNamespaceName("NonExistentNamespace"));
		}

		[Test]
		public static void CanFindContainingTypes()
		{
			var allAssemblySymbols = TestTools.GetAllFixtureAssemblySymbols();
			Assert.IsTrue(allAssemblySymbols.ContainsTypeName("ClosedGenericBehaviour"));
			Assert.IsTrue(allAssemblySymbols.ContainsTypeName("CreatureBehaviour"));
			Assert.IsTrue(allAssemblySymbols.ContainsTypeName("CreatureType"));
			Assert.IsTrue(allAssemblySymbols.ContainsTypeName("GenericBehaviour"));
			Assert.IsTrue(allAssemblySymbols.ContainsTypeName("EditorClass"));
			Assert.IsTrue(allAssemblySymbols.ContainsTypeName("PackageClass"));
			Assert.IsTrue(allAssemblySymbols.ContainsTypeName("RuntimeClass"));

			var csharpAssemblySymbol = allAssemblySymbols.First(x => x.Name == "Assembly-CSharp");
			Assert.IsTrue(csharpAssemblySymbol.ContainsTypeName("ClosedGenericBehaviour"));
			Assert.IsTrue(csharpAssemblySymbol.ContainsTypeName("CreatureBehaviour"));
			Assert.IsTrue(csharpAssemblySymbol.ContainsTypeName("CreatureType"));
			Assert.IsTrue(csharpAssemblySymbol.ContainsTypeName("GenericBehaviour"));

			Assert.IsFalse(csharpAssemblySymbol.ContainsTypeName("EditorClass"));
			Assert.IsFalse(csharpAssemblySymbol.ContainsTypeName("PackageClass"));
			Assert.IsFalse(csharpAssemblySymbol.ContainsTypeName("RuntimeClass"));

			var runtimeAssemblySymbol = allAssemblySymbols.First(x => x.Name == "Runtime");
			Assert.IsFalse(runtimeAssemblySymbol.ContainsTypeName("ClosedGenericBehaviour"));
			Assert.IsFalse(runtimeAssemblySymbol.ContainsTypeName("CreatureBehaviour"));
			Assert.IsFalse(runtimeAssemblySymbol.ContainsTypeName("CreatureType"));
			Assert.IsFalse(runtimeAssemblySymbol.ContainsTypeName("GenericBehaviour"));
			Assert.IsFalse(runtimeAssemblySymbol.ContainsTypeName("EditorClass"));
			Assert.IsFalse(runtimeAssemblySymbol.ContainsTypeName("PackageClass"));

			Assert.IsTrue(runtimeAssemblySymbol.ContainsTypeName("RuntimeClass"));

			var editorAssemblySymbol = allAssemblySymbols.First(x => x.Name == "Editor");
			Assert.IsFalse(editorAssemblySymbol.ContainsTypeName("ClosedGenericBehaviour"));
			Assert.IsFalse(editorAssemblySymbol.ContainsTypeName("CreatureBehaviour"));
			Assert.IsFalse(editorAssemblySymbol.ContainsTypeName("CreatureType"));
			Assert.IsFalse(editorAssemblySymbol.ContainsTypeName("GenericBehaviour"));

			Assert.IsTrue(editorAssemblySymbol.ContainsTypeName("EditorClass"));

			Assert.IsFalse(editorAssemblySymbol.ContainsTypeName("PackageClass"));
			Assert.IsFalse(editorAssemblySymbol.ContainsTypeName("RuntimeClass"));

			var packageAssemblySymbol = allAssemblySymbols.First(x => x.Name == "Package");
			Assert.IsFalse(packageAssemblySymbol.ContainsTypeName("ClosedGenericBehaviour"));
			Assert.IsFalse(packageAssemblySymbol.ContainsTypeName("CreatureBehaviour"));
			Assert.IsFalse(packageAssemblySymbol.ContainsTypeName("CreatureType"));
			Assert.IsFalse(packageAssemblySymbol.ContainsTypeName("GenericBehaviour"));
			Assert.IsFalse(packageAssemblySymbol.ContainsTypeName("EditorClass"));

			Assert.IsTrue(packageAssemblySymbol.ContainsTypeName("PackageClass"));

			Assert.IsFalse(packageAssemblySymbol.ContainsTypeName("RuntimeClass"));
		}

		[Test]
		public static void CanDetermineFriendAccess()
		{
			var allAssemblySymbols = TestTools.GetAllFixtureAssemblySymbols();
			var csharpAssemblySymbol = allAssemblySymbols.First(x => x.Name == "Assembly-CSharp");
			var editorAssemblySymbol = allAssemblySymbols.First(x => x.Name == "Editor");

			Assert.IsTrue(editorAssemblySymbol.IsSameAssemblyOrHasFriendAccessTo(csharpAssemblySymbol));
			Assert.IsTrue(csharpAssemblySymbol.IsSameAssemblyOrHasFriendAccessTo(csharpAssemblySymbol));
			Assert.IsFalse(csharpAssemblySymbol.IsSameAssemblyOrHasFriendAccessTo(editorAssemblySymbol));
		}
	}
}
