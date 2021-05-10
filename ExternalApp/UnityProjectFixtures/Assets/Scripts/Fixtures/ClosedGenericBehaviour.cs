namespace Fixtures
{
	public sealed class ClosedGenericBehaviour : GenericBehaviour<int>,
	                                             IFooB
	{
		public void FooB() { }
		public override void Bar() { }
	}
}
