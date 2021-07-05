using System;
using System.Runtime.InteropServices;

namespace Genesis.Plugin
{
	/// <summary>
	/// Helper methods for dealing with operating systems.
	/// </summary>
	public class OperatingSystemTools
	{
		public static OperatingSystem GetOperatingSystem()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				return OperatingSystem.macOS;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				return OperatingSystem.Linux;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return OperatingSystem.Windows;
			}

			throw new Exception("Cannot determine operating system!");
		}
	}
}
