using System.Linq;
using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
	[Category(TestConstants.CATEGORY_CODE_ANALYSIS)]
	public static class ISymbolExtensionTests
	{
		[Test]
		public static void CanDetectUnderlyingNullableType()
		{
			var classMemberTypeSymbol = TestTools.GetClassMembersTypeSymbol();
			var allMembers = classMemberTypeSymbol.GetMembers();
			var nullableInt1Field = allMembers.First(x => x.Name == "nullableInt1");
			var underlyingType = nullableInt1Field.GetNullableUnderlyingType();

			Assert.AreEqual("int", underlyingType.ToString());
		}
	}
}
