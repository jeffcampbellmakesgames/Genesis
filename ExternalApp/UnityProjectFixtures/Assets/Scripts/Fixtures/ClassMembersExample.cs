using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fixtures
{
	public class ClassMembersExample
	{
		// Properties
		public int? NullableInt1 => nullableInt1;
		public Nullable<int> NullableInt2 => nullableInt2;

		// Fields
		public int? nullableInt1;
		public Nullable<int> nullableInt2;

		public GenericBehaviour<List<ScriptableObject>> multipleGenerics;

		// Methods
		public int? NullableMethod() => nullableInt1;
	}
}
