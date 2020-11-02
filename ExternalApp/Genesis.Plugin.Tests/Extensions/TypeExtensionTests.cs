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
