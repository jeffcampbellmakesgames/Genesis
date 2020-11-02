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
