using System;

namespace Fixtures
{
	[AttributeUsage(AttributeTargets.Class)]
	public class FooDerivedAttribute : BaseAttribute
	{
		/// <inheritdoc />
		public FooDerivedAttribute() : base(AttributeType.Foo)
		{

		}
	}
}
