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

			Debug.Assert(assembly != null);

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
