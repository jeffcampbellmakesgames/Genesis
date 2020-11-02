using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
	public class TypeSymbolExtensionsTests
	{
		[Test]
		public void FullTypeNameCanBeFoundForSimpleType()
		{
			var typeSymbol = TestTools.GetSimpleTypeSymbol();

			Assert.AreEqual("Fixtures.CreatureBehaviour", typeSymbol.GetFullTypeName());
		}

		[Test]
		public void HumanReadableNameCanBeFoundForSimpleType()
		{
			var typeSymbol = TestTools.GetSimpleTypeSymbol();

			Assert.AreEqual("CreatureBehaviour", typeSymbol.GetHumanReadableName());
		}

		[Test]
		public void FullTypeNameCanBeFoundForGenericType()
		{
			var typeSymbol = TestTools.GetOpenGenericTypeSymbol();

			Assert.AreEqual("Fixtures.GenericBehaviour<T>", typeSymbol.GetFullTypeName());
		}

		[Test]
		public void HumanReadableNameIsCorrectForOpenGenericType()
		{
			var typeSymbol = TestTools.GetOpenGenericTypeSymbol();

			Assert.AreEqual("GenericBehaviourT", typeSymbol.GetHumanReadableName());
		}

		[Test]
		public void HumanReadableNameIsCorrectForClosedGenericType()
		{
			var typeSymbol = TestTools.GetClosedGenericTypeSymbol();

			Assert.AreEqual("ListGameObject", typeSymbol.GetHumanReadableName());
		}

		[Test]
		public void HumanReadableNameIsCorrectForArrayType()
		{
			var typeSymbol = TestTools.GetArrayTypeSymbol();

			Assert.AreEqual("GameObjectArray", typeSymbol.GetHumanReadableName());
		}

		[Test]
		public void IsArrayTypeWorksCorrectly()
		{
			var normalTypeSymbol = TestTools.GetSimpleTypeSymbol();

			Assert.IsFalse(normalTypeSymbol.IsArrayType());

			var arrayTypeSymbol = TestTools.GetArrayTypeSymbol();

			Assert.IsTrue(arrayTypeSymbol.IsArrayType());
		}

		[Test]
		public void IsGenericTypeWorksCorrectly()
		{
			var normalTypeSymbol = TestTools.GetSimpleTypeSymbol();

			Assert.IsFalse(normalTypeSymbol.IsGenericType());

			var genericTypeSymbol = TestTools.GetOpenGenericTypeSymbol();

			Assert.IsTrue(genericTypeSymbol.IsGenericType());
		}
	}
}
