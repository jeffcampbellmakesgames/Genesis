using UnityEngine;

namespace Fixtures
{
	public abstract class GenericBehaviour<T> : MonoBehaviour,
												IFooA
	{
		public void FooA() { }
		public virtual void Bar() { }
	}
}
