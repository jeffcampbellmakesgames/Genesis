using System.Collections.Generic;
using Genesis.Unity.Factory;
using UnityEngine;

namespace Fixtures
{
	[FactoryKeyFor(typeof(Sprite))]
	[FactoryKeyFor(typeof(GameObject[]))]
	[FactoryKeyFor(typeof(List<GameObject>))]
	[FactoryKeyFor(typeof(ScriptableObject))]
	public class CreatureBehaviour : MonoBehaviour
	{

	}
}
