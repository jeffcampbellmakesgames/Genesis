using System.Collections.Generic;
using Genesis.Unity.Factory;
using UnityEngine;

namespace ExampleContent
{
	[FactoryKeyFor(typeof(Sprite))]
	[FactoryKeyFor(typeof(GameObject))]
	[FactoryKeyFor(typeof(GameObject[]))]
	[FactoryKeyFor(typeof(List<GameObject>))]
	[CreateAssetMenu]
	public sealed class ArchetypeID : ScriptableObject { }
}
