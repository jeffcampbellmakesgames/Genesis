using System;
using System.Collections.Generic;
using System.Linq;
using Genesis.Unity.Factory.Plugin;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace Genesis.Plugin.Tests.DataProviders
{
	[TestFixture]
	internal class ScriptableFactoryDataProviderTests
	{
		private IReadOnlyList<INamedTypeSymbol> _typeSymbols;
		private IMemoryCache _memoryCache;
		private List<CodeGeneratorData> _codeGeneratorData;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_typeSymbols = TestTools.GetAllFixtureTypeSymbols();
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
