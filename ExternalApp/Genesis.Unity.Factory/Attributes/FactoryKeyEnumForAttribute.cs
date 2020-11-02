using System;

namespace Genesis.Unity.Factory
{
	/// <summary>
	/// Indicates that a scriptable factory should be created using the decorated enum as a key to
	/// <see cref="ValueType"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Enum, AllowMultiple = true)]
	public sealed class FactoryKeyEnumForAttribute : Attribute
	{
		public Type ValueType { get; }

		public FactoryKeyEnumForAttribute(Type valueType)
		{
			ValueType = valueType;
		}
	}
}
