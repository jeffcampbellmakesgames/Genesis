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

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Extension methods for <see cref="string"/>.
	/// </summary>
	public static class StringExtension
	{
		public static string UppercaseFirst(this string str)
		{
			return string.IsNullOrEmpty(str) ? str : char.ToUpper(str[0]) + str.Substring(1);
		}

		public static string LowercaseFirst(this string str)
		{
			return string.IsNullOrEmpty(str) ? str : char.ToLower(str[0]) + str.Substring(1);
		}

		public static string ToUnixLineEndings(this string str)
		{
			return str.Replace("\r\n", "\n").Replace("\r", "\n");
		}

		public static string ToWindowsLineEndings(this string str)
		{
			return str.Replace("\n", "\r\n");
		}

		public static string ToUnixPath(this string str)
		{
			return str.Replace("\\", "/");
		}

		public static string ToCSV(this string[] values)
		{
			return string.Join(", ", values.Where(value => !string.IsNullOrEmpty(value)).Select(value => value.Trim()).ToArray());
		}

		public static string[] ArrayFromCSV(this string values)
		{
			return values.Split(
					new char[1]
					{
						','
					},
					StringSplitOptions.RemoveEmptyEntries)
				.Select(value => value.Trim())
				.ToArray();
		}

		public static string ToSpacedCamelCase(this string text)
		{
			var stringBuilder = new StringBuilder(text.Length * 2);
			stringBuilder.Append(char.ToUpper(text[0]));
			for (var index = 1; index < text.Length; ++index)
			{
				if (char.IsUpper(text[index]) && text[index - 1] != ' ')
				{
					stringBuilder.Append(' ');
				}

				stringBuilder.Append(text[index]);
			}

			return stringBuilder.ToString();
		}

		public static string MakePathRelativeTo(this string path, string currentDirectory)
		{
			currentDirectory = currentDirectory.CreateUri();
			path = path.CreateUri();
			if (path.StartsWith(currentDirectory))
			{
				path = path.Replace(currentDirectory, string.Empty);
				if (path.StartsWith("/"))
				{
					path = path.Substring(1);
				}
			}

			return path;
		}

		public static string CreateUri(this string path)
		{
			var uri = new Uri(path);
			return Uri.UnescapeDataString(uri.AbsolutePath + uri.Fragment);
		}

		/// <summary>
		/// Returns true if <paramref name="value"/> is null or empty, otherwise false.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		/// <summary>
		/// Returns true if <paramref name="value"/> is a valid filename, otherwise false.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsValidFileName(this string value)
		{
			return value.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
		}

		public static string ToSafeDirectory(this string directory)
		{
			if (string.IsNullOrEmpty(directory) || directory == ".")
			{
				return "Generated";
			}

			if (directory.EndsWith("/", StringComparison.Ordinal))
			{
				directory = directory.Substring(0, directory.Length - 1);
			}

			if (!directory.EndsWith("/Generated", StringComparison.OrdinalIgnoreCase))
			{
				directory += "/Generated";
			}

			return directory;
		}
	}
}
