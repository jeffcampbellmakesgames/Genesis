using System;

namespace Fixtures
{
	public class CloneableClass : ICloneable
	{
		/// <inheritdoc />
		public object Clone()
		{
			return new CloneableClass();
		}
	}
}
