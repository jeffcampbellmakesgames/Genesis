using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;

namespace Genesis.CLI
{
	/// <summary>
	/// An assembly loader that allows for assemblies to be loaded and resolved for the purpose of read-only
	/// inspection of meta-information. Any attempt to inspect types in such a way that would cause instantiation
	/// will most-likely fail and/or result in an exception.
	/// </summary>
	internal sealed class ReflectionOnlyAssemblyLoader : MetadataAssemblyResolver, IDisposable
	{
		private readonly ILogger _logger;
		private readonly HashSet<Assembly> _assemblies;
		private readonly PathAssemblyResolver _resolver;
		private readonly MetadataLoadContext _context;

		public ReflectionOnlyAssemblyLoader(IEnumerable<string> assemblyPaths)
		{
			_logger = Log.Logger.ForContext<ReflectionOnlyAssemblyLoader>();
			_assemblies = new HashSet<Assembly>();
			_resolver = new PathAssemblyResolver(GetAllAssemblyPaths(assemblyPaths));
			_context = new MetadataLoadContext(this, GetCoreAssemblyName());
		}

		/// <summary>
		/// Returns a collection of all types for inspection (reflection only).
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Type> GetTypes()
		{
			return _context.GetAssemblies().SelectMany(x => x.GetTypes());
		}

		public void Load(string assemblyPath)
		{
			TryLoadAssembly(assemblyPath);
		}

		public void LoadAll(IEnumerable<string> assemblyPaths)
		{
			foreach (var assemblyPath in assemblyPaths)
			{
				TryLoadAssembly(assemblyPath);
			}
		}

		private void TryLoadAssembly(string assemblyPath)
		{
			var fullPath = Path.GetFullPath(assemblyPath);
			if (File.Exists(fullPath))
			{
				_logger.Verbose($"Loading Assembly [{fullPath}] for Reflection-Only.");

				_assemblies.Add(_context.LoadFromAssemblyPath(fullPath));
			}
			else if (Directory.Exists(fullPath))
			{
				var searchAssemblyPaths = Directory.GetFiles(
					fullPath,
					FileConstants.WILDCARD_DLL_SEARCH_FILTER,
					SearchOption.AllDirectories);

				foreach (var matchingPluginPath in searchAssemblyPaths)
				{
					_logger.Verbose($"Loading Assembly [{matchingPluginPath}] for Reflection-Only.");

					_assemblies.Add(_context.LoadFromAssemblyPath(matchingPluginPath));
				}
			}
		}

		private IEnumerable<string> GetAllAssemblyPaths(IEnumerable<string> assemblyPaths)
		{
			// Get a list of all assemblies
			var pathList = new List<string>();
			foreach (var assemblyPath in assemblyPaths)
			{
				var fullPath = Path.GetFullPath(assemblyPath);
				if (File.Exists(fullPath))
				{
					pathList.Add(fullPath);
				}
				else if (Directory.Exists(fullPath))
				{
					var searchAssemblyPaths = Directory.GetFiles(
						fullPath,
						FileConstants.WILDCARD_DLL_SEARCH_FILTER,
						SearchOption.AllDirectories);

					foreach (var matchingPluginPath in searchAssemblyPaths)
					{
						pathList.Add(matchingPluginPath);
					}
				}
			}

			pathList.Add(GetCoreAssemblyFullPath());
			pathList.AddRange(Directory.GetFiles(GetCoreAssemblyDirectoryPath(), FileConstants.WILDCARD_DLL_SEARCH_FILTER));

			var currentAssemblyDirectories = AppDomain.CurrentDomain.GetAssemblies()
				.Select(
					x =>
					{
						if (x.IsDynamic || string.IsNullOrEmpty(x.Location))
						{
							return string.Empty;
						}

						var fileInfo = new FileInfo(x.Location);
						return fileInfo.Directory != null
							? fileInfo.Directory.FullName
							: string.Empty;
					})
				.Where(x => !string.IsNullOrEmpty(x))
				.Distinct()
				.ToArray();

			foreach (var dir in currentAssemblyDirectories)
			{
				pathList.AddRange(Directory.GetFiles(dir, FileConstants.WILDCARD_DLL_SEARCH_FILTER, SearchOption.AllDirectories));
			}

			var finalPathsList = pathList.Distinct().ToArray();

			return finalPathsList;
		}

		private static string GetCoreAssemblyFullPath()
		{
			// Get full paths to all currently executing assemblies and the core assembly.
			var coreAssemblyFullPath = typeof(object).GetTypeInfo().Assembly.Location;
			var coreAssemblyFileInfo = new FileInfo(coreAssemblyFullPath);

			Debug.Assert(coreAssemblyFileInfo.Directory != null, "coreAssemblyFileInfo.Directory != null");

			return coreAssemblyFileInfo.FullName;
		}

		private static string GetCoreAssemblyDirectoryPath()
		{
			// Get full paths to all currently executing assemblies and the core assembly.
			var coreAssemblyFullPath = typeof(object).GetTypeInfo().Assembly.Location;
			var coreAssemblyFileInfo = new FileInfo(coreAssemblyFullPath);

			Debug.Assert(coreAssemblyFileInfo.Directory != null, "coreAssemblyFileInfo.Directory != null");

			return coreAssemblyFileInfo.Directory.FullName;
		}

		private static string GetCoreAssemblyName()
		{
			// Get full paths to all currently executing assemblies and the core assembly.
			var coreAssemblyFullPath = typeof(object).GetTypeInfo().Assembly.Location;
			var coreAssemblyFileInfo = new FileInfo(coreAssemblyFullPath);

			Debug.Assert(coreAssemblyFileInfo.Directory != null, "coreAssemblyFileInfo.Directory != null");

			return coreAssemblyFileInfo.Name.Replace(FileConstants.DLL_EXTENSION, string.Empty);
		}

		#region IDisposable

		public void Dispose()
		{
			_context?.Dispose();
		}

		#endregion

		public override Assembly Resolve(MetadataLoadContext context, AssemblyName assemblyName)
		{
			_logger.Verbose($"Resolving Assembly [{assemblyName}] for Reflection-Only.");

			if (_assemblies.Any(x => x.GetName().Name == assemblyName.Name))
			{
				_logger.Verbose($"Returning cached assembly [{assemblyName}].");

				return _assemblies.First(x => x.GetName().Name == assemblyName.Name);
			}

			return _resolver.Resolve(context, assemblyName);
		}
	}
}
