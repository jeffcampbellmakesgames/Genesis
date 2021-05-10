using System;
using System.Collections.Generic;
using Genesis.Unity.Factory;
using UnityEngine;

namespace Fixtures
{
	[Serializable]
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
