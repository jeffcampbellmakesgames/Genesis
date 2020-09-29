using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Genesis.Shared
{
	/// <summary>
	/// Helper-methods for dealing with reflection.
	/// </summary>
	public static class ReflectionTools
	{
		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfType<T>() where T : class
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
								  !myType.IsAbstract &&
								  !myType.IsGenericType &&
								  myType.IsSubclassOf(typeof(T))))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfType<T>(string[] whitelistOfAssemblies)
			where T : class
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          myType.IsSubclassOf(typeof(T))))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T and decorated with TV.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TV"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfTypeWithAttribute<T, TV>()
			where T : class, new()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
								  !myType.IsAbstract &&
								  !myType.IsGenericType &&
								  myType.IsSubclassOf(typeof(T)) &&
								  myType.IsDefined(typeof(TV), true)))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T and decorated with TV.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TV"></typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfTypeWithAttribute<T, TV>(string[] whitelistOfAssemblies)
			where T : class, new()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          myType.IsSubclassOf(typeof(T)) &&
						          myType.IsDefined(typeof(TV), true)))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Type T that implement interface V
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <typeparam name="TV">The Type a class must implement</typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllInstancesOfTypeWithInterface<T, TV>()
			where T : class, new()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
								  !myType.IsAbstract &&
								  !myType.IsGenericType &&
								  (myType.IsAssignableFrom(typeof(T)) || myType.IsSubclassOf(typeof(T))) &&
								  myType.GetInterfaces().Contains(typeof(TV))))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Type T that implement interface V
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <typeparam name="TV">The Type a class must implement</typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetAllInstancesOfTypeWithInterface<T, TV>(string[] whitelistOfAssemblies)
			where T : class, new()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          (myType.IsAssignableFrom(typeof(T)) || myType.IsSubclassOf(typeof(T))) &&
						          myType.GetInterfaces().Contains(typeof(TV))))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Type T that implement interface V
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <typeparam name="TV">The Attribute a class must have</typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllInstancesOfTypeWithAttribute<T, TV>()
			where T : class, new()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
								  !myType.IsAbstract &&
								  !myType.IsGenericType &&
								  (myType.IsAssignableFrom(typeof(T)) || myType.IsSubclassOf(typeof(T))) &&
								  myType.IsDefined(typeof(TV), true)))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Type T that implement interface V
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <typeparam name="TV">The Attribute a class must have</typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetAllInstancesOfTypeWithAttribute<T, TV>(string[] whitelistOfAssemblies)
			where T : class, new()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          (myType.IsAssignableFrom(typeof(T)) || myType.IsSubclassOf(typeof(T))) &&
						          myType.IsDefined(typeof(TV), true)))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T that implement V.
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <typeparam name="TV">the Type a class must implement</typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfTypeWithInterface<T, TV>()
			where T : class, new()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
								  !myType.IsAbstract &&
								  !myType.IsGenericType &&
								  myType.IsSubclassOf(typeof(T)) &&
								  myType.GetInterfaces().Contains(typeof(TV))))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T that implement V.
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <typeparam name="TV">the Type a class must implement</typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfTypeWithInterface<T, TV>(string[] whitelistOfAssemblies)
			where T : class, new()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          myType.IsSubclassOf(typeof(T)) &&
						          myType.GetInterfaces().Contains(typeof(TV))))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types that implement V.
		/// </summary>
		/// <typeparam name="T">the Type a class must implement</typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllImplementingInstancesOfInterface<T>()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          myType.GetInterfaces().Contains(typeof(T))))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types that implement V.
		/// </summary>
		/// <typeparam name="T">the Type a class must implement</typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetAllImplementingInstancesOfInterface<T>(string[] whitelistOfAssemblies)
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          myType.GetInterfaces().Contains(typeof(T))))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types contained in <paramref name="types"/> that implement V.
		/// </summary>
		/// <typeparam name="T">the Type a class must implement</typeparam>
		/// <param name="types">The Type[] to search through</param>
		public static IEnumerable<T> GetAllImplementingInstancesOfInterface<T>(IEnumerable<Type> types)
		{
			var objects = new List<T>();
			foreach (var type in types
				.Where(
					myType => myType.IsClass &&
					          !myType.IsAbstract &&
					          !myType.IsGenericType &&
					          myType.GetInterfaces().Contains(typeof(T))))
			{
				objects.Add((T)Activator.CreateInstance(type));
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of Types contained in <paramref name="types"/> that implement
		/// <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">the Type a class must implement</typeparam>
		/// <param name="types">The Type[] to search through</param>
		public static IEnumerable<Type> GetAllImplementingTypesOfInterface<T>(IEnumerable<Type> types)
		{
			var objects = new List<Type>();
			foreach (var type in types
				.Where(
					myType => myType.IsClass &&
					          !myType.IsAbstract &&
					          !myType.IsGenericType &&
					          myType.GetInterfaces().Contains(typeof(T))))
			{
				objects.Add(type);
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of Types that implement <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">the Type a class must implement</typeparam>
		public static IEnumerable<Type> GetAllImplementingTypesOfInterface<T>()
		{
			var objects = new List<Type>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          myType.GetInterfaces().Contains(typeof(T))))
				{
					objects.Add(type);
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of Types that implement <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">the Type a class must implement</typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		public static IEnumerable<Type> GetAllImplementingTypesOfInterface<T>(string[] whitelistOfAssemblies)
		{
			var objects = new List<Type>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          myType.GetInterfaces().Contains(typeof(T))))
				{
					objects.Add(type);
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T that take arguments constructorArgs
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <param name="constructorArgs"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfType<T>(params object[] constructorArgs)
			where T : class, new()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
								  !myType.IsAbstract &&
								  !myType.IsGenericType &&
								  myType.IsSubclassOf(typeof(T))))
				{
					objects.Add((T)Activator.CreateInstance(type, constructorArgs));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T that take arguments constructorArgs
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		/// <param name="constructorArgs"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfType<T>(string[] whitelistOfAssemblies, params object[] constructorArgs)
			where T : class, new()
		{
			var objects = new List<T>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          myType.IsSubclassOf(typeof(T))))
				{
					objects.Add((T)Activator.CreateInstance(type, constructorArgs));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllDerivedTypesOfType<T>()
			where T : class
		{
			var objects = new List<Type>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
								  myType.IsSubclassOf(typeof(T))))
				{
					objects.Add(type);
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllDerivedTypesOfType<T>(string[] whitelistOfAssemblies)
			where T : class
		{
			var objects = new List<Type>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          myType.IsSubclassOf(typeof(T))))
				{
					objects.Add(type);
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <typeparam name="TV"></typeparam>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllDerivedTypesOfTypeWithAttribute<T, TV>(bool inherit = true)
			where T : class where TV : Attribute
		{
			var objects = new List<Type>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly
					.GetTypes()
					.Where(
						myType => myType.IsClass &&
								  myType.IsSubclassOf(typeof(T)) &&
								  myType.IsDefined(typeof(TV), inherit)))
				{
					objects.Add(type);
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <typeparam name="TV"></typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllDerivedTypesOfTypeWithAttribute<T, TV>(string[] whitelistOfAssemblies, bool inherit = true)
			where T : class
			where TV : Attribute
		{
			var objects = new List<Type>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly
					.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          myType.IsSubclassOf(typeof(T)) &&
						          myType.IsDefined(typeof(TV), inherit)))
				{
					objects.Add(type);
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T that implement V.
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <typeparam name="TV">the Type a class must implement</typeparam>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllDerivedTypesWithInterface<T, TV>()
			where T : class
		{
			var objects = new List<Type>();
			foreach (var assembly in GetAvailableAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
								  !myType.IsAbstract &&
								  !myType.IsGenericType &&
								  myType.IsSubclassOf(typeof(T)) &&
								  myType.GetInterfaces().Contains(typeof(TV))))
				{
					objects.Add(type);
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of class instances of Types derived from T that implement V.
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from. </typeparam>
		/// <typeparam name="TV">the Type a class must implement. </typeparam>
		/// <param name = "whitelistOfAssemblies">The array of assemblies types must belong to.</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllDerivedTypesWithInterface<T, TV>(string[] whitelistOfAssemblies)
			where T : class
		{
			var objects = new List<Type>();
			foreach (var assembly in GetAvailableAssemblies(whitelistOfAssemblies))
			{
				foreach (var type in assembly.GetTypes()
					.Where(
						myType => myType.IsClass &&
						          !myType.IsAbstract &&
						          !myType.IsGenericType &&
						          myType.IsSubclassOf(typeof(T)) &&
						          myType.GetInterfaces().Contains(typeof(TV))))
				{
					objects.Add(type);
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of all loaded <see cref="Assembly"/> instances in <see cref="AppDomain"/>
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Assembly> GetAvailableAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		/// <summary>
		/// Returns an IEnumerable of all loaded <see cref="Assembly"/> instances in <see cref="AppDomain"/> where the
		/// name of the <see cref="Assembly"/> has been specified in the <paramref name="whitelistOfAssemblies"/>.
		/// </summary>
		/// <param name = "whitelistOfAssemblies">
		/// The array of assemblies that whitelist what assemblies are returned in the IEnumerable.
		/// </param>
		/// <returns></returns>
		public static IEnumerable<Assembly> GetAvailableAssemblies(string[] whitelistOfAssemblies)
		{
			Debug.Assert(whitelistOfAssemblies.Length != 0);

			return AppDomain.CurrentDomain.GetAssemblies().Where(x => whitelistOfAssemblies.Contains(x.GetName().Name));
		}
	}
}
