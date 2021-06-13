/*

MIT License

Copyright (c) Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;

namespace Genesis.CLI
{
	/// <summary>
	/// A loader class that allows for assemblies to be loaded and any dependencies resolved. This allows for
	/// inspection and instantiation of types from loaded assemblies as long as any and all required
	/// dependencies can be resolved.
	/// </summary>
	public sealed class AssemblyLoader : IReadOnlyCollection<Assembly>,
	                                     IDisposable
	{
		/// <summary>
		/// Defines a plugin constraint that should determine whether or not another assembly should be loaded.
		/// </summary>
		private class PluginConstraint
		{
			public AssemblyName AssemblyName { get; }

			public PluginConstraint(AssemblyName assemblyName)
			{
				AssemblyName = assemblyName;
			}

			/// <summary>
			/// Returns true if <paramref name="otherAssemblyName"/> matches the assembly this constraint is
			/// regarding, otherwise false.
			/// </summary>
			public bool AppliesTo(AssemblyName otherAssemblyName)
			{
				return AssemblyName.Name != null && AssemblyName.Name.Equals(otherAssemblyName.Name);
			}

			/// <summary>
			/// Returns true if the version of <paramref name="otherAssemblyName"/> is out of date with this plugin
			/// constraint based on matching major and minor versions, otherwise false.
			/// </summary>
			public bool WithinVersionConstraint(AssemblyName otherAssemblyName)
			{
				if (otherAssemblyName.Version == null || AssemblyName.Version == null)
				{
					return true;
				}

				var pluginVersion = AssemblyName.Version;
				var otherVersion = otherAssemblyName.Version;
				return otherVersion.Major == pluginVersion.Major &&
				       otherVersion.Minor == pluginVersion.Minor;
			}

			/// <summary>
			/// Returns true if the version of <paramref name="otherAssemblyName"/> matches the version of this
			/// constraint exactly.
			/// </summary>
			public bool HasSameVersion(AssemblyName otherAssemblyName)
			{
				return otherAssemblyName.Version == AssemblyName.Version;
			}
		}

		private bool _forceLoadAllUnsafePlugins;

		private readonly ILogger _logger;
		private readonly IEnumerable<string> _basePaths;
		private readonly HashSet<Assembly> _assemblies;
		private readonly AppDomain _appDomain;
		private readonly List<PluginConstraint> _pluginConstraints;

		public AssemblyLoader(string basePath) : this(new []{basePath})
		{
		}

		public AssemblyLoader(IEnumerable<string> basePaths)
		{
			_logger = Log.Logger.ForContext<AssemblyLoader>();
			_assemblies = new HashSet<Assembly>();
			_appDomain = AppDomain.CurrentDomain;
			_basePaths = basePaths;
			_pluginConstraints = new List<PluginConstraint>();
			_appDomain.AssemblyResolve += OnAssemblyResolve;
		}

		/// <summary>
		/// Adds a plugin constraint for Assembly with <paramref name="assemblyName"/>.
		/// </summary>
		public void AddPluginConstraint(AssemblyName assemblyName)
		{
			if (_pluginConstraints.Any(x => x.AssemblyName == assemblyName))
			{
				return;
			}

			_pluginConstraints.Add(new PluginConstraint(assemblyName));
		}

		/// <summary>
		/// Sets a flag to attempt to load all unsafe plugins regardless of whether or not plugin constraints are met.
		/// </summary>
		public void ForceUnsafeAssemblyLoad()
		{
			_forceLoadAllUnsafePlugins = true;
		}

		/// <summary>
		/// Loads all assemblies at <paramref name="assemblyDiscoverPath"/>.
		/// </summary>
		public void LoadAll(string assemblyDiscoverPath)
		{
			LoadAll(new []{assemblyDiscoverPath});
		}

		/// <summary>
		/// Loads all assemblies at <paramref name="assemblyDiscoverPaths"/>.
		/// </summary>
		public void LoadAll(IEnumerable<string> assemblyDiscoverPaths)
		{
			var assemblyPaths = new List<string>();
			foreach (var assemblyDiscoverPath in assemblyDiscoverPaths)
			{
				var fullPath = Path.GetFullPath(assemblyDiscoverPath);
				if (File.Exists(fullPath))
				{
					assemblyPaths.Add(fullPath);
				}
				else if (Directory.Exists(fullPath))
				{
					var searchAssemblyPaths = Directory.GetFiles(
						fullPath,
						FileConstants.WILDCARD_DLL_SEARCH_FILTER,
						SearchOption.AllDirectories);

					foreach (var matchingPluginPath in searchAssemblyPaths)
					{
						assemblyPaths.Add(matchingPluginPath);
					}
				}
			}

			foreach (var assemblyPath in assemblyPaths)
			{
				var fullPath = Path.GetFullPath(assemblyPath);

                _logger.Verbose("Attempting to load Assembly [{AssemblyPath}].", fullPath);

				try
				{
					// Get assembly
					var assembly = ResolveAndLoad(fullPath, false);
					var assemblyName = assembly.GetName();

					// Get all assembly names for referenced assemblies and itself.
					var referenceAssemblyNames = assembly.GetReferencedAssemblies()
						.Concat(new []{assembly.GetName()});

					var meetsAllPluginConstraints = true;
					foreach (var referencedAssemblyName in referenceAssemblyNames)
					{
						foreach (var pluginConstraint in _pluginConstraints)
						{
							if (pluginConstraint.AppliesTo(referencedAssemblyName) &&
							    !pluginConstraint.HasSameVersion(referencedAssemblyName))
							{
								if (pluginConstraint.WithinVersionConstraint(referencedAssemblyName))
								{
									const string WITH_VERSION_CONSTRAINT_LOG =
										"Assembly {AssemblyName} at v{AssemblyVersion} references an older version " +
										"of {ReferenceAssemblyName} at v{ReferenceAssemblyVersion}, but can still be " +
										"loaded.";

									_logger.Warning(WITH_VERSION_CONSTRAINT_LOG,
										assemblyName.Name,
										assemblyName.Version,
										referencedAssemblyName.Name,
										referencedAssemblyName.Version);
								}
								else if(_forceLoadAllUnsafePlugins)
								{
									const string OUT_OF_DATE_CONSTRAINT_LOG =
										"Assembly {AssemblyName} at v{AssemblyVersion} references an out-of-date " +
										"version of {ReferenceAssemblyName} at v{ReferenceAssemblyVersion}, but will be " +
										"loaded anyways as \"Load-Unsafe\" is set to true.";

									_logger.Warning(OUT_OF_DATE_CONSTRAINT_LOG,
										assemblyName.Name,
										assemblyName.Version,
										referencedAssemblyName.Name,
										referencedAssemblyName.Version);
								}
								else
								{

									const string OUT_OF_DATE_CONSTRAINT_LOG =
										"Assembly {AssemblyName} at v{AssemblyVersion} references an out-of-date " +
										"version of {ReferenceAssemblyName} at v{ReferenceAssemblyVersion} and won't " +
										"be loaded.";

									_logger.Warning(OUT_OF_DATE_CONSTRAINT_LOG,
										assemblyName.Name,
										assemblyName.Version,
										referencedAssemblyName.Name,
										referencedAssemblyName.Version);
									meetsAllPluginConstraints = false;
								}
							}
						}
					}

					// If the assembly we're trying to load meets all plugin constraints
					if (meetsAllPluginConstraints)
					{
						AddAssembly(assembly);
					}
				}
				catch (Exception ex)
				{
					const string FAILED_TO_LOAD_ASSEMBLY_LOG =
						"Failed to load assembly [{AssemblyPath}], unexpected error occurred. See exception for more " +
						"details.";

					_logger.Error(
						ex,
						FAILED_TO_LOAD_ASSEMBLY_LOG,
						fullPath);
				}
			}
		}

		private Assembly ResolveAndLoad(string name, bool isDependency)
		{
			Assembly assembly = null;
			try
			{
				_logger.Verbose(isDependency
					? "Loading Assembly Dependency [{AssemblyDependency}]"
					: "Loading Assembly [{Assembly}]",
					name);

				assembly = Assembly.LoadFrom(name);
			}
			catch (Exception ex1)
			{
				_logger.Error(
					ex1,
					"Failed to load Assembly [{Assembly}], attempting to resolve Assembly at Base Paths.",
					name);

				var resolvedPath = ResolvePath(name);
				if (!string.IsNullOrEmpty(resolvedPath))
				{
					try
					{
						_logger.Verbose(
							"Attempting to resolve Assembly [{Assembly}] at path [{AssemblyResolvedPath}]",
							name,
							resolvedPath);

						assembly = Assembly.LoadFrom(resolvedPath);
					}
					catch (BadImageFormatException ex2)
					{
						const string BAD_IMAGE_LOG =
							"Failed to load Assembly [{Assembly}], the file format doesn't conform to the expected " +
							"language/runtime.";
						_logger.Error(
							ex2,
							BAD_IMAGE_LOG,
							name);
					}
					catch (Exception ex3)
					{
						_logger.Error(ex3, "Failed to load Assembly [{Assembly}].", name);
					}
				}
			}

			return assembly;
		}

		private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			return ResolveAndLoad(args.Name, true);
		}

		private string ResolvePath(string name)
		{
			try
			{
				var assemblyName = new AssemblyName(name).Name;

                _logger.Verbose("Computed Assembly Name is [{AssemblyName}]", assemblyName);

				if (!assemblyName.EndsWith(FileConstants.DLL_EXTENSION, StringComparison.OrdinalIgnoreCase) &&
					!assemblyName.EndsWith(FileConstants.EXE_EXTENSION, StringComparison.OrdinalIgnoreCase))
				{
					assemblyName += FileConstants.DLL_EXTENSION;
				}

				foreach (var basePath in _basePaths)
				{
					var fullPath = Path.GetFullPath(Path.Combine(basePath, assemblyName));
					_logger.Verbose("Looking for Assembly at [{AssemblyPath}].", fullPath);

					if (File.Exists(fullPath))
					{
						_logger.Verbose("Resolved Assembly [{AssemblyName}] to [{AssemblyPath}].", name, fullPath);
						return fullPath;
					}
				}
			}
			catch (FileLoadException ex)
			{
				_logger.Verbose(ex, "Could not resolve [{AssemblyName}] to a valid Assembly name.", name);
			}

			return null;
		}

		public IEnumerable<Type> GetTypes()
		{
			return _assemblies.SelectMany(x => x.GetTypes());
		}

		#region IEnumerable<Assembly>

		public IEnumerator<Assembly> GetEnumerator()
		{
			return _assemblies.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			_appDomain.AssemblyResolve -= OnAssemblyResolve;
		}

		#endregion

		private void AddAssembly(Assembly assembly)
		{
			if (assembly != null)
			{
				_assemblies.Add(assembly);
				_appDomain.Load(assembly.GetName());
			}
		}

		#region Implementation of IReadOnlyCollection<out Assembly>

		/// <summary>
		/// Gets the number of elements in the collection.
		/// </summary>
		/// <returns>The number of elements in the collection.</returns>
		public int Count => _assemblies.Count;

		#endregion
	}
}
