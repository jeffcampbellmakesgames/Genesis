using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
	[Category(TestConstants.CATEGORY_CODE_ANALYSIS)]
	public static class UnityExtensionsTests
	{
		[Test]
		public static void CanDetectUnityObject()
		{
			var scriptableObjectTypeSymbol = TestTools.GetUnityScriptableObjectTypeSymbol();
			var monobehaviorObjectTypeSymbol = TestTools.GetUnityMonoBehaviourTypeSymbol();
			var pocoTypeSymbol = TestTools.GetCloneableClassTypeSymbol();

			Assert.IsTrue(scriptableObjectTypeSymbol.IsUnityObject());
			Assert.IsTrue(monobehaviorObjectTypeSymbol.IsUnityObject());
			Assert.IsFalse(pocoTypeSymbol.IsUnityObject());
		}

		[Test]
		public static void CanDetectUnityScriptableObject()
		{
			var scriptableObjectTypeSymbol = TestTools.GetUnityScriptableObjectTypeSymbol();
			var monobehaviorObjectTypeSymbol = TestTools.GetUnityMonoBehaviourTypeSymbol();
			var pocoTypeSymbol = TestTools.GetCloneableClassTypeSymbol();

			Assert.IsTrue(scriptableObjectTypeSymbol.IsUnityScriptableObject());
			Assert.IsFalse(monobehaviorObjectTypeSymbol.IsUnityScriptableObject());
			Assert.IsFalse(pocoTypeSymbol.IsUnityScriptableObject());
		}

		[Test]
		public static void CanDetectUnityGameObject()
		{
			var gameObjectTypeSymbol = TestTools.GetUnityGameObjectTypeSymbol();
			var scriptableObjectTypeSymbol = TestTools.GetUnityScriptableObjectTypeSymbol();
			var monobehaviorObjectTypeSymbol = TestTools.GetUnityMonoBehaviourTypeSymbol();
			var pocoTypeSymbol = TestTools.GetCloneableClassTypeSymbol();

			Assert.IsTrue(gameObjectTypeSymbol.IsUnityGameObject());
			Assert.IsFalse(scriptableObjectTypeSymbol.IsUnityGameObject());
			Assert.IsFalse(monobehaviorObjectTypeSymbol.IsUnityGameObject());
			Assert.IsFalse(pocoTypeSymbol.IsUnityGameObject());
		}

		[Test]
		public static void CanDetectUnityMonobehaviour()
		{
			var scriptableObjectTypeSymbol = TestTools.GetUnityScriptableObjectTypeSymbol();
			var monobehaviorObjectTypeSymbol = TestTools.GetUnityMonoBehaviourTypeSymbol();
			var pocoTypeSymbol = TestTools.GetCloneableClassTypeSymbol();

			Assert.IsFalse(scriptableObjectTypeSymbol.IsUnityMonoBehaviour());
			Assert.IsTrue(monobehaviorObjectTypeSymbol.IsUnityMonoBehaviour());
			Assert.IsFalse(pocoTypeSymbol.IsUnityMonoBehaviour());
		}
	}
}
