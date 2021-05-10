using NUnit.Framework;

namespace Genesis.Plugin.Tests
{
	[TestFixture]
	public static class UnityExtensionsTests
	{
		[Test]
		public static void CanDetectUnityMonoBehaviour()
		{
			var exampleMonoBehaviourTypeSymbol = TestTools.GetUnityMonoBehaviourTypeSymbol();

			Assert.IsTrue(exampleMonoBehaviourTypeSymbol.IsMonoBehaviour());
		}

		[Test]
		public static void CanDetectUnityScriptableObject()
		{
			var exampleScriptableObjectTypeSymbol = TestTools.GetUnityScriptableObjectTypeSymbol();

			Assert.IsTrue(exampleScriptableObjectTypeSymbol.IsScriptableObject());
		}
	}
}
