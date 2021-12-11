using Genesis.Unity.Factory;
using UnityEngine;

namespace ExampleContent
{
	[GenerateSymbolFactory]
	public class BehaviourSymbolObject : MonoBehaviour,
										 ISymbolObject
	{
		/// <inheritdoc />
		public string Symbol => _symbol;

		[SerializeField]
		private string _symbol;
	}
}
