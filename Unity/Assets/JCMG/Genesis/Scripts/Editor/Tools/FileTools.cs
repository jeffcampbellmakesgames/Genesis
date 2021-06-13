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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.SharpZipLib.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Helper methods for dealing with files/directories
	/// </summary>
	internal static class FileTools
	{
		/// <summary>
		/// Converts <paramref name="fullFilePath"/> into a relative file path from <paramref name="referencePath"/>.
		/// </summary>
		public static string ConvertToRelativePath(string fullFilePath, string referencePath)
		{
			var fileUri = new Uri(fullFilePath);
			var referenceUri = new Uri(referencePath);
			return referenceUri.MakeRelativeUri(fileUri).ToString();
		}

		/// <summary>
		/// Returns an absolute path to the Unity Project folder.
		/// </summary>
		public static string GetProjectPath()
		{
			const string ASSETS_FOLDER_NAME = "Assets";
			return Path.GetFullPath(Application.dataPath.Replace(ASSETS_FOLDER_NAME, string.Empty));
		}

		/// <summary>
		/// Returns true if a single C# solution file is found, otherwise false.
		/// </summary>
		public static bool HasSingleSolutionFile()
		{
			return GetSolutionPaths().Length == 1;
		}

		/// <summary>
		/// Returns the absolute file paths to the first solution file in the project directory.
		/// </summary>
		public static string GetFirstSolutionPath()
		{
			var solutionPaths = GetSolutionPaths();

			Assert.IsTrue(solutionPaths.Any());

			return solutionPaths[0];
		}

		/// <summary>
		/// Returns the absolute file paths to all solution files in the project directory.
		/// </summary>
		public static string[] GetSolutionPaths()
		{
			return Directory.GetFiles(GetProjectPath(), EditorConstants.WILDCARD_ALL_SLNS);
		}

		/// <summary>
		/// Returns the absolute paths to all assemblies in the Unity Project whether in the Assets folder or packages.
		/// </summary>
		public static string[] GetAssemblyPaths()
		{
			var assemblyPathsList = new List<string>();
			assemblyPathsList.AddRange(GetPackageAssembliesPath());
			assemblyPathsList.AddRange(GetAssetPathsWithDLLs());

			const string RUNTIME_ASSEMBLIES_PATH = @"Managed\UnityEngine";
			assemblyPathsList.Add(
				Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, RUNTIME_ASSEMBLIES_PATH)));

			const string LIBRARY_SCRIPT_ASSEMBLIES_PATH = @"Library\ScriptAssemblies";
			assemblyPathsList.Add(Path.GetFullPath(Path.Combine(GetProjectPath(), LIBRARY_SCRIPT_ASSEMBLIES_PATH)));

			return assemblyPathsList.Distinct().ToArray();
		}

		/// <summary>
		/// Returns the absolute folder paths to all assemblies in packages referenced by this project.
		/// </summary>
		public static string[] GetPackageAssembliesPath()
		{
			const string ROOT_PACKAGES_PATH = @"Library\PackageCache\";

			var fullLibraryPath = Path.Combine(GetProjectPath(), ROOT_PACKAGES_PATH);
			var dllPaths = FindAllDLLs(fullLibraryPath);
			return dllPaths.Select(x => new FileInfo(x)).Select(y => y.Directory.FullName).Distinct().ToArray();
		}

		/// <summary>
		/// Returns the absolute folder paths to all assemblies in the Assets folder.
		/// </summary>
		public static string[] GetAssetPathsWithDLLs()
		{
			var dllPaths = FindAllDLLs(Application.dataPath);
			return dllPaths.Select(x => new FileInfo(x)).Select(y => y.Directory.FullName).Distinct().ToArray();
		}

		/// <summary>
		/// Recursively searches for all assemblies in <paramref name="rootPath"/> and subfolders.
		/// </summary>
		private static string[] FindAllDLLs(string rootPath)
		{
			return Directory.GetFiles(rootPath, EditorConstants.WILDCARD_ALL_DLLS, SearchOption.AllDirectories);
		}

		/// <summary>
		/// Copies directory/file contents recursively from <paramref name="source"/> to <paramref name="target"/>.
		/// </summary>
		public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
		{
			// Recursively copy all directories first, then copy all files.
			foreach (DirectoryInfo dir in source.GetDirectories())
			{
				CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
			}

			foreach (FileInfo file in source.GetFiles())
			{
				file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite:true);
			}
		}

		/// <summary>
		/// Extracts the contents of the zip file at <paramref name="fullZipPath"/> to the
		/// <paramref name="destinationPath"/>.
		/// </summary>
		public static void ExtractZipContents(string fullZipPath, string destinationPath)
		{
			Assert.IsTrue(File.Exists(fullZipPath));
			Assert.IsTrue(Directory.Exists(destinationPath));

			// Zip Extraction
			const string TEMP_EXTRACT_PATH = "Temp/GenesisZipExtraction";

			var tempExtractPath = Path.GetFullPath(Path.Combine(GetProjectPath(), TEMP_EXTRACT_PATH));

			ZipUtility.UncompressFromZip(
				fullZipPath,
				string.Empty,
				tempExtractPath);

			var sourceDirectory = new DirectoryInfo(tempExtractPath);
			var targetDirectory = new DirectoryInfo(destinationPath);

			CopyFilesRecursively(sourceDirectory, targetDirectory);

			sourceDirectory.Delete(recursive: true);
		}
	}
}
