using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Genesis.Shared
{
	/// <summary>
	/// Extension methods for <see cref="string"/>.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Converts an array of <see cref="string"/> to a comma-delimited string array.
		/// </summary>
		public static string ToCSV(this string[] values)
		{
			return string.Join(", ", values.Where(value => !string.IsNullOrEmpty(value)).Select(value => value.Trim()).ToArray());
		}

		/// <summary>
		/// Converts a comma-delimited string array to an array of strings.
		/// </summary>
		public static string[] ArrayFromCSV(this string values)
		{
			return values.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries)
				.Select(value => value.Trim())
				.ToArray();
		}

		/// <summary>
		/// Formats a string path for the target directory so that it is ready for writing files to.
		/// </summary>
		public static string ToSafeDirectory(this string directory)
		{
			const string OUTPUT_FOLDER_NAME = "Generated";

			if (string.IsNullOrEmpty(directory) || directory == ".")
			{
				return OUTPUT_FOLDER_NAME;
			}

			var folderName = new DirectoryInfo(directory).Name;
			if (folderName != OUTPUT_FOLDER_NAME)
			{
				directory = Path.Combine(directory, OUTPUT_FOLDER_NAME);
			}

			return directory.Replace('\\', '/');
		}

		/// <summary>
		/// Returns a Base64-encoded <see cref="string"/>.
		/// </summary>
		public static string ConvertToBase64(this string str)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
		}

		/// <summary>
		/// Returns a Base64-decoded <see cref="string"/>.
		/// </summary>
		public static string ConvertFromBase64(this string base64Str)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(base64Str));
		}
	}
}
