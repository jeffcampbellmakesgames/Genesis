using System.Collections.Generic;
using Genesis.Unity.Factory;
using UnityEngine;

namespace Fixtures
{
	[FactoryKeyEnumFor(typeof(Sprite))]
	[FactoryKeyEnumFor(typeof(GameObject))]
	[FactoryKeyEnumFor(typeof(GameObject[]))]
	[FactoryKeyEnumFor(typeof(List<GameObject>))]
	public enum CreatureType
	{
		Human,
		Orc,
		Goblin
	}
}
