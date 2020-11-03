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
using Genesis.Shared;
using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
	[Category(TestConstants.CATEGORY_CODE_ANALYSIS)]
	public class AssembliesConfigExtensionsTests
	{
		private IGenesisConfig _genesisConfig;
		private AssembliesConfig _assembliesConfig;

		[SetUp]
		public void SetUp()
		{
			_genesisConfig = new GenesisConfig();
			_assembliesConfig = _genesisConfig.CreateAndConfigure<AssembliesConfig>();
			_assembliesConfig.RawWhiteListedAssemblies = string.Empty;
		}

		[Test]
		public void NothingIsFilteredWhenWhiteListingIsNotEnabled()
		{
			var namedTypeSymbols = TestTools.GetAllFixtureTypeSymbols();
			var filteredNameTypeSymbols = _assembliesConfig.FilterTypeSymbols(namedTypeSymbols);

			Assert.AreEqual(namedTypeSymbols, filteredNameTypeSymbols);
		}

		[Test]
		public void AllTypesAreFilteredWhenWhiteListingIsEnabledAndZeroAssembliesPresent()
		{
			_assembliesConfig.DoUseWhitelistOfAssemblies = true;

			var namedTypeSymbols = TestTools.GetAllFixtureTypeSymbols();
			var filteredNameTypeSymbols = _assembliesConfig.FilterTypeSymbols(namedTypeSymbols);

			Assert.AreNotEqual(namedTypeSymbols, filteredNameTypeSymbols);
			Assert.IsEmpty(filteredNameTypeSymbols);
		}

		[Test]
		public void TypesAreFilteredWhenWhiteListingIsEnabledAssembliesPresent()
		{
			_assembliesConfig.DoUseWhitelistOfAssemblies = true;
			_assembliesConfig.RawWhiteListedAssemblies = "Assembly-CSharp,Runtime";

			var namedTypeSymbols = TestTools.GetAllFixtureTypeSymbols();
			var filteredNameTypeSymbols = _assembliesConfig.FilterTypeSymbols(namedTypeSymbols);

			Assert.AreNotEqual(namedTypeSymbols, filteredNameTypeSymbols);

			// These types should be filtered out.
			Assert.IsFalse(filteredNameTypeSymbols.Any(x => x.Name == "PackageClass"));
			Assert.IsFalse(filteredNameTypeSymbols.Any(x => x.Name == "EditorClass"));

			// This type should still be present.
			Assert.IsTrue(filteredNameTypeSymbols.Any(x => x.Name == "CSharpAssemblyClass"));
			Assert.IsTrue(filteredNameTypeSymbols.Any(x => x.Name == "RuntimeClass"));
		}
	}
}
