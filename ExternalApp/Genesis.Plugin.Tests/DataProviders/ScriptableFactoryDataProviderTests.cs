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

using Genesis.Shared;
using Genesis.Unity.Factory.Plugin;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genesis.Plugin.Tests.DataProviders
{
	[TestFixture]
	[Category(TestConstants.CATEGORY_CODE_ANALYSIS)]
	internal class ScriptableFactoryDataProviderTests
	{
		private IReadOnlyList<ICachedNamedTypeSymbol> _typeSymbols;
		private IMemoryCache _memoryCache;
		private List<CodeGeneratorData> _codeGeneratorData;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_typeSymbols = TestTools.GetAllFixtureTypeSymbols().Select(CachedNamedTypeSymbol.Create).ToList();
		}

		[SetUp]
		public void SetUp()
		{
			_memoryCache = new MemoryCache();
			_memoryCache.AddNamedTypeSymbols(_typeSymbols);
		}

		[TearDown]
		public void TearDown()
		{
			_memoryCache.Clear();
		}

		[Test]
		public void CanDiscoverFactoryDecoratedTypes()
		{
			var factoryDataProvider = new ScriptableFactoryDataProvider();
			factoryDataProvider.SetCache(_memoryCache);
			factoryDataProvider.Configure(new GenesisConfig());

			_codeGeneratorData = new List<CodeGeneratorData>(factoryDataProvider.GetData());

			foreach (var codeGenData in _codeGeneratorData)
			{
				if (codeGenData is FactoryKeyData factoryKeyData)
				{
					var keyTypeName = factoryKeyData.GetKeyHumanReadableName();
					var keyFullTypeName = factoryKeyData.GetKeyFullTypeName();

					var valueTypeName = factoryKeyData.GetValueHumanReadableName();
					var valueFullTypeName = factoryKeyData.GetValueFullTypeName();

					Console.WriteLine($"{keyFullTypeName} => {keyTypeName}, {valueFullTypeName} => {valueTypeName}");
				}
				else if (codeGenData is FactoryKeyEnumData factoryKeyEnumData)
				{
					var keyTypeName = factoryKeyEnumData.GetKeyHumanReadableName();
					var keyFullTypeName = factoryKeyEnumData.GetKeyFullTypeName();

					var valueTypeName = factoryKeyEnumData.GetValueHumanReadableName();
					var valueFullTypeName = factoryKeyEnumData.GetValueFullTypeName();

					Console.WriteLine($"{keyFullTypeName} => {keyTypeName}, {valueFullTypeName} => {valueTypeName}");
				}
			}

			var creatureTypeCodeGenData = _codeGeneratorData.OfType<FactoryKeyEnumData>().FirstOrDefault(x =>
				x.GetValueFullTypeName() == "UnityEngine.GameObject");

			Assert.IsNotNull(creatureTypeCodeGenData);
			Assert.AreEqual("CreatureType", creatureTypeCodeGenData.GetKeyHumanReadableName());
			Assert.AreEqual("Fixtures.CreatureType", creatureTypeCodeGenData.GetKeyFullTypeName());
			Assert.AreEqual("GameObject", creatureTypeCodeGenData.GetValueHumanReadableName());
			Assert.AreEqual("UnityEngine.GameObject", creatureTypeCodeGenData.GetValueFullTypeName());

			var creatureBehaviourCodeGenData = _codeGeneratorData.OfType<FactoryKeyData>().FirstOrDefault(x =>
				x.GetValueFullTypeName() == "UnityEngine.ScriptableObject");

			Assert.IsNotNull(creatureBehaviourCodeGenData);
			Assert.AreEqual("CreatureBehaviour", creatureBehaviourCodeGenData.GetKeyHumanReadableName());
			Assert.AreEqual("Fixtures.CreatureBehaviour", creatureBehaviourCodeGenData.GetKeyFullTypeName());
			Assert.AreEqual("ScriptableObject", creatureBehaviourCodeGenData.GetValueHumanReadableName());
			Assert.AreEqual("UnityEngine.ScriptableObject", creatureBehaviourCodeGenData.GetValueFullTypeName());
		}
	}
}
