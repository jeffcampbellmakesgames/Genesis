using System;
using Genesis.Unity.Factory;
using UnityEngine;

namespace ExampleContent
{
	[GenerateSymbolFactory]
	[Serializable]
	public class POCOSymbolObject : ISymbolObject
	{
		/// <inheritdoc />
		public string Symbol => _symbol;

		[SerializeField]
		private string _symbol;

		[SerializeField]
		private GameObject _gameObject;
	}
}
