using Genesis.Shared;
using NUnit.Framework;
using UnityEngine;

namespace JCMG.Genesis.Editor.Tests
{
	[TestFixture]
	internal class SerializationTests
	{
		private GenesisConfig _genesisConfig;

		[SetUp]
		public void SetUp()
		{
			_genesisConfig = new GenesisConfig();
		}

		[Test]
		public void VerifyConfigCanBeSerializedToAndFromJson()
		{
			// Add several key-value-pairs
			_genesisConfig.SetValue("one", "1");
			_genesisConfig.SetValue("two", "2");
			_genesisConfig.SetValue("three", "3");

			var json = JsonUtility.ToJson(_genesisConfig);

			const string EXPECTED_JSON =
				"{\"keyValuePairs\":[{\"key\":\"one\",\"value\":\"1\"}," +
				"{\"key\":\"two\",\"value\":\"2\"}," +
				"{\"key\":\"three\",\"value\":\"3\"}],\"name\":\"DefaultGenesisConfig\"}";

			Assert.AreEqual(EXPECTED_JSON, json);

			var newGenesisConfig = JsonUtility.FromJson<GenesisConfig>(json);

			// Verify keys exist
			Assert.IsTrue(newGenesisConfig.HasValue("one"));
			Assert.IsTrue(newGenesisConfig.HasValue("two"));
			Assert.IsTrue(newGenesisConfig.HasValue("three"));

			// Verify values are as expected
			Assert.AreEqual("1", newGenesisConfig.GetOrSetValue("one"));
			Assert.AreEqual("2", newGenesisConfig.GetOrSetValue("two"));
			Assert.AreEqual("3", newGenesisConfig.GetOrSetValue("three"));
		}
	}
}
