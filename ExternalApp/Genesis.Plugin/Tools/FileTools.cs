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
using System.IO;
using System.Linq;

namespace Genesis.Plugin
{
	/// <summary>
	///     Helper methods for dealing with files/directories
	/// </summary>
	public static class FileTools
	{
		/// <summary>
		///     Converts <paramref name="fullFilePath" /> into a relative file path from <paramref name="referencePath" />.
		/// </summary>
		/// <param name="fullFilePath"></param>
		/// <param name="referencePath"></param>
		/// <returns></returns>
		public static string ConvertToRelativePath(string fullFilePath, string referencePath)
		{
			var fileUri = new Uri(fullFilePath);
			var referenceUri = new Uri(referencePath);
			return referenceUri.MakeRelativeUri(fileUri).ToString();
		}

		/// <summary>
		///     Returns true if the <paramref name="path" /> is for a file, otherwise false.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static bool IsFile(string path)
		{
			var attr = File.GetAttributes(path);
			return (attr & FileAttributes.Directory) != FileAttributes.Directory;
		}

		/// <summary>
		///     Recursively deletes all sub-folders (excluding hidden folders) and files in <see cref="DirectoryInfo" />
		///     <paramref name="directoryInfo" />.
		/// </summary>
		/// <param name="directoryInfo"></param>
		private static void RecursivelyDeleteDirectoryContents(DirectoryInfo directoryInfo)
		{
			var subDirectoryInfo = directoryInfo.GetDirectories("*");
			foreach (var sdi in subDirectoryInfo)
				if ((sdi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
					sdi.Delete(true);

			var fileInfo = directoryInfo.GetFiles("*");
			foreach (var fi in fileInfo) fi.Delete();
		}

		public static bool ContainsFile(DirectoryInfo directoryInfo, string absoluteFilePath)
		{
			var file = Directory.GetFiles(
					directoryInfo.FullName,
					absoluteFilePath,
					SearchOption.AllDirectories)
				.FirstOrDefault();

			return file != null;
		}
	}
}
