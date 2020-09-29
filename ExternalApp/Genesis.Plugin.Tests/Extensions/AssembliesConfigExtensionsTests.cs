using System.Linq;
using Genesis.Shared;
using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
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
