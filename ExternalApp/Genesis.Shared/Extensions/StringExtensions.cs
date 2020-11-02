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
