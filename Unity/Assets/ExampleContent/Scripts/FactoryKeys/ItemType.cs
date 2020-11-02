using System.Collections.Generic;
using Genesis.Unity.Factory;
using UnityEngine;

namespace ExampleContent
{
	/// <summary>
	/// An example enum that can take advantage of a code generator to create a scriptable factory
	/// for several arbitrary types.
	/// </summary>
	[FactoryKeyEnumFor(typeof(Sprite))]
	[FactoryKeyEnumFor(typeof(GameObject))]
	[FactoryKeyEnumFor(typeof(GameObject[]))]
	[FactoryKeyEnumFor(typeof(List<GameObject>))]
	public enum ItemType
	{
		Sword,
		Axe,
		Shield,
		Potion
	}
}
