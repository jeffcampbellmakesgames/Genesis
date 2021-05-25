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

using System.Diagnostics;
using System.Reflection;

namespace Genesis.CLI
{
	/// <summary>
	/// A constants class whose values are calculated and baked into an internal class at compile-time.
	/// </summary>
	internal static class VersionConstants
	{
		public static readonly string MAJOR;
		public static readonly string MINOR;
		public static readonly string PATCH;
		public static readonly string ASSEMBLY_SEM_VER;
		public static readonly string ASSEMBLY_SEM_FILE_VER;
		public static readonly string ASSEMBLY_INFO_VERSION;

		static VersionConstants()
		{
			var assembly = Assembly.GetAssembly(typeof(VersionConstants));
			var assemblyName = assembly.GetName();
			var assemblySemVer = assemblyName.Version;
			var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

			MAJOR = assemblySemVer.Major.ToString();
			MINOR = assemblySemVer.Minor.ToString();
			PATCH = assemblySemVer.Build.ToString();
			ASSEMBLY_SEM_VER = assemblySemVer.ToString();
			ASSEMBLY_SEM_FILE_VER = fileVersionInfo.FileVersion;
			ASSEMBLY_INFO_VERSION = fileVersionInfo.ProductVersion;
		}
	}
}
