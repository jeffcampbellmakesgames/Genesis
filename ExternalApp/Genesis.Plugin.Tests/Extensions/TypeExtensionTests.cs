using System.Collections.Generic;
using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
	internal sealed class TypeExtensionTests
	{
		#region Fixtures

		private interface IFoo
		{
		}

		private class BadFoo
		{
		}

		private class GoodFoo : IFoo
		{
		}

		#endregion

		[Test]
		public void CanDetermineIfTypeImplementsInterface()
		{
			Assert.IsFalse(typeof(BadFoo).ImplementsInterface<IFoo>());
			Assert.IsTrue(typeof(GoodFoo).ImplementsInterface<IFoo>());
		}

		[Test]
		public void SafeTypeNameCanBeFound()
		{
			Assert.AreEqual("GoodFooArray", typeof(GoodFoo[]).GetHumanReadableName());
			Assert.AreEqual("ListGoodFoo", typeof(List<GoodFoo>).GetHumanReadableName());
			Assert.AreEqual("DictionaryIntGoodFoo", typeof(Dictionary<int, GoodFoo>).GetHumanReadableName());
		}

		[Test]
		public void FullTypeNameCanBeFound()
		{
			Assert.AreEqual(
				"Genesis.Plugin.Tests.TypeExtensionTests.GoodFoo[]",
				typeof(GoodFoo[]).GetFullTypeName());

			Assert.AreEqual(
				"System.Collections.Generic.List<Genesis.Plugin.Tests.TypeExtensionTests.GoodFoo>",
				typeof(List<GoodFoo>).GetFullTypeName());

			Assert.AreEqual(
				"System.Collections.Generic.Dictionary<System.Int32,Genesis.Plugin.Tests.TypeExtensionTests.GoodFoo>",
				typeof(Dictionary<int, GoodFoo>).GetFullTypeName());
		}
	}
}
