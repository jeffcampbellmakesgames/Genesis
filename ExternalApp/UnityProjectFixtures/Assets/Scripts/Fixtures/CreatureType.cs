using System;
using System.Collections.Generic;
using Genesis.Unity.Factory;
using UnityEngine;

namespace Fixtures
{
	[Serializable]
	[FactoryKeyEnumFor(typeof(Sprite))]
	[FactoryKeyEnumFor(typeof(string))]
	[FactoryKeyEnumFor(typeof(GameObject))]
	[FactoryKeyEnumFor(typeof(GameObject[]))]
	[FactoryKeyEnumFor(typeof(GameObject[,]))]
	[FactoryKeyEnumFor(typeof(List<GameObject>))]
	[FactoryKeyEnumFor(typeof(List<GameObject>))]
	[FactoryKeyEnumFor(typeof(Dictionary<int, GameObject>))]
	public enum CreatureType
	{
		Human,
		Orc,
		Goblin
	}
}
