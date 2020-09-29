using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Genesis.Shared;

namespace Genesis.Plugin
{
	/// <summary>
	/// A cache of types based on the configuration set through its methods.
	/// </summary>
	public sealed class TypeCache : IReadOnlyCollection<Type>
	{
		public int Count => _types.Count;

		public Type this[int index] => _types[index];

		/// <summary>
		/// Anything that is or derives from these types is not included
		/// </summary>
		private readonly List<Type> _ignoreAttributeTypes = new List<Type>();

		/// <summary>
		/// Anything that is or derives from these types is not included
		/// </summary>
		private readonly List<Type> _ignoreClassTypes = new List<Type>();

		/// <summary>
		/// All cached types
		/// </summary>
		private readonly List<Type> _types = new List<Type>();

		public void AddType<T>() where T : class
		{
			var types = ReflectionTools.GetAllDerivedTypesOfType<T>();

			foreach (var type in types)
			{
				if (_ignoreClassTypes.Any(x => type.IsSubclassOf(x) || type == x))
				{
					continue;
				}

				if (_ignoreAttributeTypes.Any(x => type.GetCustomAttributes(x, true).Length > 0))
				{
					continue;
				}

				_types.Add(type);
			}
		}

		public void AddTypeWithAttribute<T, TV>() where T : class where TV : Attribute
		{
			var types = ReflectionTools.GetAllDerivedTypesOfTypeWithAttribute<T, TV>(inherit:false);
			foreach (var type in types)
			{
				if (_ignoreClassTypes.Any(x => type.IsSubclassOf(x) || type == x))
				{
					continue;
				}

				if (_ignoreAttributeTypes.Any(x => type.GetCustomAttributes(x, false).Length > 0))
				{
					continue;
				}

				_types.Add(type);
			}
		}

		public void AddTypeWithInterface<T, TV>() where T : class where TV : class
		{
			if (!typeof(TV).IsInterface)
			{
				throw new ArgumentException($"Type {typeof(TV)} cannot be added as it is not an Interface.");
			}

			var types = ReflectionTools.GetAllDerivedTypesWithInterface<T, TV>();
			foreach (var type in types)
			{
				if (_ignoreClassTypes.Any(x => type.IsSubclassOf(x) || type == x))
				{
					continue;
				}

				if (_ignoreAttributeTypes.Any(x => type.GetCustomAttributes(x, false).Length > 0))
				{
					continue;
				}

				_types.Add(type);
			}
		}

		/// <summary>
		/// Add a type to be excluded from caching even if it meets other AddType qualifications
		/// This type will be ignored if a type that is attempted to be added is a subclass of this type or
		/// is this type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void IgnoreType<T>() where T : class
		{
			if (!_ignoreClassTypes.Contains(typeof(T)))
			{
				_ignoreClassTypes.Add(typeof(T));
			}
		}

		/// <summary>
		/// Add a type to be excluded from caching even if it meets other AddType qualifications
		/// This type will be ignored if a type that is attempted to be added is a subclass of this type or
		/// is this type.
		/// </summary>
		/// <param name="type"></param>
		public void IgnoreType(Type type)
		{
			if (!type.IsClass)
			{
				throw new ArgumentException($"Type {type} cannot be ignored as it is not a class.");
			}

			_ignoreClassTypes.Add(type);
		}

		/// <summary>
		/// Adds an attribute on a class to be excluded from caching even if it meets other
		/// AddType qualifications.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void IgnoreAttribute<T>() where T : Attribute
		{
			if (!_ignoreAttributeTypes.Contains(typeof(T)))
			{
				_ignoreAttributeTypes.Add(typeof(T));
			}
		}

		public void Clear()
		{
			_types.Clear();
		}

		#region IEnumerable

		public IEnumerator<Type> GetEnumerator()
		{
			return _types.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
