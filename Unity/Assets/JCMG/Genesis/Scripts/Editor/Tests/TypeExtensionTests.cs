using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace JCMG.Genesis.Editor.Tests
{
	[TestFixture]
	internal sealed class TypeExtensionTests
	{
		#region Fixtures

		private interface IFoo { }
		private class BadFoo { }
		private class GoodFoo : IFoo { }

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
				"UnityEngine.GameObject[]",
				typeof(GameObject[]).GetFullTypeName());

			Assert.AreEqual(
				"System.Collections.Generic.List<UnityEngine.GameObject>",
				typeof(List<GameObject>).GetFullTypeName());

			Assert.AreEqual(
				"System.Collections.Generic.Dictionary<System.Int32,UnityEngine.GameObject>",
				typeof(Dictionary<int, GameObject>).GetFullTypeName());
		}
	}
}
