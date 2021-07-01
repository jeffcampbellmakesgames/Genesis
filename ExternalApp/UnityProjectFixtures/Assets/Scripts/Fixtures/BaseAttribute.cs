using System;

namespace Fixtures
{
	public enum AttributeType
	{
		Foo,
		Bar
	}


	public abstract class BaseAttribute : Attribute
	{
		public AttributeType Type { get; }

		protected BaseAttribute(AttributeType type)
		{
			Type = type;
		}
	}
}
