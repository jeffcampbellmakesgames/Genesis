using System;

namespace Fixtures
{
	[AttributeUsage(AttributeTargets.Class)]
	public class BarDerivedAttribute : BaseAttribute
	{
		/// <inheritdoc />
		public BarDerivedAttribute() : base(AttributeType.Bar)
		{
		}
	}
}
