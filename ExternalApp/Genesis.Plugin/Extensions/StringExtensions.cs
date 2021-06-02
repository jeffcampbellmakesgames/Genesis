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
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;

namespace Genesis.Plugin
{
	/// <summary>
	///     Extension methods for <see cref="string" />.
	/// </summary>
	public static class StringExtensions
	{
		private static ImmutableArray<string> s_lazyNumerals;

		private static readonly Func<char, char> s_toLower = char.ToLower;
		private static readonly Func<char, char> s_toUpper = char.ToUpper;

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

		public static string ToSpacedCamelCase(this string text)
		{
			var stringBuilder = new StringBuilder(text.Length * 2);
			stringBuilder.Append(char.ToUpper(text[0]));
			for (var index = 1; index < text.Length; ++index)
			{
				if (char.IsUpper(text[index]) && text[index - 1] != ' ') stringBuilder.Append(' ');

				stringBuilder.Append(text[index]);
			}

			return stringBuilder.ToString();
		}

		public static string MakePathRelativeTo(this string path, string currentDirectory)
		{
			currentDirectory = currentDirectory.CreateUri();
			path = path.CreateUri();
			if (path.StartsWith(currentDirectory, StringComparison.Ordinal))
			{
				path = path.Replace(currentDirectory, string.Empty);
				if (path.StartsWith("/", StringComparison.Ordinal))
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
		///     Returns true if <paramref name="value" /> is null or empty, otherwise false.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		/// <summary>
		///     Returns true if <paramref name="value" /> is a valid filename, otherwise false.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsValidFileName(this string value)
		{
			return value.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
		}

		/// <summary>
		///		Returns true if this represents a global namespace otherwise false.
		/// </summary>
		public static bool IsGlobalNamespace(this string fullNamespace)
		{
			return string.IsNullOrEmpty(fullNamespace) ||
			       fullNamespace == CodeGenerationConstants.GLOBAL_NAMESPACE;
		}

		/// <summary>
		///     Returns the short name of the type (type name without namespace
		/// </summary>
		public static string GetShortTypeName(this string fullTypeName)
		{
			var shortTypeName = fullTypeName.Split(".").Last();
			if (shortTypeName.Contains(CodeGenerationConstants.LEFT_CHEVRON_CHAR))
			{
				shortTypeName = shortTypeName.Remove(CodeGenerationConstants.LEFT_CHEVRON_CHAR);
			}

			return shortTypeName;
		}

		public static int? GetFirstNonWhitespaceOffset(this string line)
		{
			for (var i = 0; i < line.Length; i++)
			{
				if (!char.IsWhiteSpace(line[i]))
				{
					return i;
				}
			}

			return null;
		}

		public static string GetLeadingWhitespace(this string lineText)
		{
			var firstOffset = lineText.GetFirstNonWhitespaceOffset();
			return firstOffset.HasValue
				? lineText.Substring(0, firstOffset.Value)
				: lineText;
		}

		public static int GetTextColumn(this string text, int tabSize, int initialColumn)
		{
			var lineText = text.GetLastLineText();
			if (text != lineText)
			{
				return lineText.GetColumnFromLineOffset(lineText.Length, tabSize);
			}
			return text.ConvertTabToSpace(tabSize, initialColumn, text.Length) + initialColumn;
		}

		public static int ConvertTabToSpace(this string textSnippet, int tabSize, int initialColumn, int endPosition)
		{
			var column = initialColumn;

			// now this will calculate indentation regardless of actual content on the buffer except TAB
			for (var i = 0; i < endPosition; i++)
			{
				if (textSnippet[i] == '\t')
				{
					column += tabSize - column % tabSize;
				}
				else
				{
					column++;
				}
			}

			return column - initialColumn;
		}

		public static int IndexOf(this string text, Func<char, bool> predicate)
		{
			if (text == null) return -1;

			for (var i = 0; i < text.Length; i++)
			{
				if (predicate(text[i]))
				{
					return i;
				}
			}

			return -1;
		}

		public static string GetFirstLineText(this string text)
		{
			var lineBreak = text.IndexOf('\n');
			if (lineBreak < 0)
			{
				return text;
			}

			return text.Substring(0, lineBreak + 1);
		}

		public static string GetLastLineText(this string text)
		{
			var lineBreak = text.LastIndexOf('\n');
			if (lineBreak < 0)
			{
				return text;
			}

			return text.Substring(lineBreak + 1);
		}

		public static bool ContainsLineBreak(this string text)
		{
			foreach (var ch in text)
			{
				if (ch == '\n' || ch == '\r')
				{
					return true;
				}
			}

			return false;
		}

		public static int GetNumberOfLineBreaks(this string text)
		{
			var lineBreaks = 0;
			for (var i = 0; i < text.Length; i++)
			{
				if (text[i] == '\n')
				{
					lineBreaks++;
				}
				else if (text[i] == '\r')
				{
					if (i + 1 == text.Length || text[i + 1] != '\n')
					{
						lineBreaks++;
					}
				}
			}

			return lineBreaks;
		}

		public static bool ContainsTab(this string text)
		{
			// PERF: Tried replacing this with "text.IndexOf('\t')>=0", but that was actually slightly slower
			foreach (var ch in text)
			{
				if (ch == '\t')
				{
					return true;
				}
			}

			return false;
		}

		public static ImmutableArray<SymbolDisplayPart> ToSymbolDisplayParts(this string text)
		{
			return ImmutableArray.Create(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, text));
		}

		public static int GetColumnOfFirstNonWhitespaceCharacterOrEndOfLine(this string line, int tabSize)
		{
			var firstNonWhitespaceChar = line.GetFirstNonWhitespaceOffset();

			if (firstNonWhitespaceChar.HasValue)
				return line.GetColumnFromLineOffset(firstNonWhitespaceChar.Value, tabSize);
			return line.GetColumnFromLineOffset(line.Length, tabSize);
		}

		public static int GetColumnFromLineOffset(this string line, int endPosition, int tabSize)
		{
			return ConvertTabToSpace(line, tabSize, 0, endPosition);
		}

		public static int GetLineOffsetFromColumn(this string line, int column, int tabSize)
		{
			var currentColumn = 0;

			for (var i = 0; i < line.Length; i++)
			{
				if (currentColumn >= column) return i;

				if (line[i] == '\t')
					currentColumn += tabSize - currentColumn % tabSize;
				else
					currentColumn++;
			}

			// We're asking for a column past the end of the line, so just go to the end.
			return line.Length;
		}

		internal static string GetNumeral(int number)
		{
			var numerals = s_lazyNumerals;
			if (numerals.IsDefault)
			{
				numerals = ImmutableArray.Create("0", "1", "2", "3", "4", "5", "6", "7", "8", "9");
				ImmutableInterlocked.InterlockedInitialize(ref s_lazyNumerals, numerals);
			}

			Debug.Assert(number >= 0);
			return number < numerals.Length ? numerals[number] : number.ToString();
		}

		public static string Join(this IEnumerable<string> source, string separator)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			if (separator == null) throw new ArgumentNullException(nameof(separator));

			return string.Join(separator, source);
		}

		public static bool LooksLikeInterfaceName(this string name)
		{
			return name.Length >= 3 && name[0] == 'I' && char.IsUpper(name[1]) && char.IsLower(name[2]);
		}

		public static bool LooksLikeTypeParameterName(this string name)
		{
			return name.Length >= 3 && name[0] == 'T' && char.IsUpper(name[1]) && char.IsLower(name[2]);
		}

		public static string ToPascalCase(
			this string shortName,
			bool trimLeadingTypePrefix = true)
		{
			return ConvertCase(shortName, trimLeadingTypePrefix, s_toUpper);
		}

		public static string ToCamelCase(
			this string shortName,
			bool trimLeadingTypePrefix = true)
		{
			return ConvertCase(shortName, trimLeadingTypePrefix, s_toLower);
		}

		private static string ConvertCase(
			this string shortName,
			bool trimLeadingTypePrefix,
			Func<char, char> convert)
		{
			// Special case the common .net pattern of "IFoo" as a type name.  In this case we
			// want to generate "foo" as the parameter name.
			if (!string.IsNullOrEmpty(shortName))
			{
				if (trimLeadingTypePrefix &&
				    (shortName.LooksLikeInterfaceName() || shortName.LooksLikeTypeParameterName()))
					return convert(shortName[1]) + shortName.Substring(2);

				if (convert(shortName[0]) != shortName[0]) return convert(shortName[0]) + shortName.Substring(1);
			}

			return shortName;
		}

		internal static bool IsValidClrTypeName(this string name)
		{
			return !string.IsNullOrEmpty(name) && name.IndexOf('\0') == -1;
		}

		/// <summary>
		///     Checks if the given name is a sequence of valid CLR names separated by a dot.
		/// </summary>
		internal static bool IsValidClrNamespaceName(this string name)
		{
			if (string.IsNullOrEmpty(name)) return false;

			var lastChar = '.';
			foreach (var c in name)
			{
				if (c == '\0' || c == '.' && lastChar == '.') return false;

				lastChar = c;
			}

			return lastChar != '.';
		}

		internal static string GetWithSingleAttributeSuffix(
			this string name,
			bool isCaseSensitive)
		{
			var cleaned = name;
			while ((cleaned = GetWithoutAttributeSuffix(cleaned, isCaseSensitive)) != null) name = cleaned;

			return name + "Attribute";
		}

		internal static bool TryGetWithoutAttributeSuffix(
			this string name,
			out string result)
		{
			return TryGetWithoutAttributeSuffix(name, true, out result);
		}

		internal static string GetWithoutAttributeSuffix(
			this string name,
			bool isCaseSensitive)
		{
			string result;
			return TryGetWithoutAttributeSuffix(name, isCaseSensitive, out result) ? result : null;
		}

		internal static bool TryGetWithoutAttributeSuffix(
			this string name,
			bool isCaseSensitive,
			out string result)
		{
			const string AttributeSuffix = "Attribute";
			var comparison = isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			if (name.Length > AttributeSuffix.Length && name.EndsWith(AttributeSuffix, comparison))
			{
				result = name.Substring(0, name.Length - AttributeSuffix.Length);
				return true;
			}

			result = null;
			return false;
		}

		internal static bool IsValidUnicodeString(this string str)
		{
			var i = 0;
			while (i < str.Length)
			{
				var c = str[i++];

				// (high surrogate, low surrogate) makes a valid pair, anything else is invalid:
				if (char.IsHighSurrogate(c))
				{
					if (i < str.Length && char.IsLowSurrogate(str[i]))
						i++;
					else
						// high surrogate not followed by low surrogate
						return false;
				}
				else if (char.IsLowSurrogate(c))
				{
					// previous character wasn't a high surrogate
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     Remove one set of leading and trailing double quote characters, if both are present.
		/// </summary>
		internal static string Unquote(this string arg)
		{
			return Unquote(arg, out var quoted);
		}

		internal static string Unquote(this string arg, out bool quoted)
		{
			if (arg.Length > 1 && arg[0] == '"' && arg[arg.Length - 1] == '"')
			{
				quoted = true;
				return arg.Substring(1, arg.Length - 2);
			}

			quoted = false;
			return arg;
		}

		internal static int IndexOfBalancedParenthesis(this string str, int openingOffset, char closing)
		{
			var opening = str[openingOffset];

			var depth = 1;
			for (var i = openingOffset + 1; i < str.Length; i++)
			{
				var c = str[i];
				if (c == opening)
				{
					depth++;
				}
				else if (c == closing)
				{
					depth--;
					if (depth == 0) return i;
				}
			}

			return -1;
		}

		// String isn't IEnumerable<char> in the current Portable profile.
		internal static char First(this string arg)
		{
			return arg[0];
		}

		// String isn't IEnumerable<char> in the current Portable profile.
		internal static char Last(this string arg)
		{
			return arg[arg.Length - 1];
		}

		// String isn't IEnumerable<char> in the current Portable profile.
		internal static bool All(this string arg, Predicate<char> predicate)
		{
			foreach (var c in arg)
				if (!predicate(c))
					return false;

			return true;
		}

		public static string EscapeIdentifier(
			this string identifier,
			bool isQueryContext = false)
		{
			var nullIndex = identifier.IndexOf('\0');
			if (nullIndex >= 0) identifier = identifier.Substring(0, nullIndex);

			var needsEscaping = SyntaxFacts.GetKeywordKind(identifier) != SyntaxKind.None;

			// Check if we need to escape this contextual keyword
			needsEscaping = needsEscaping || isQueryContext &&
				SyntaxFacts.IsQueryContextualKeyword(SyntaxFacts.GetContextualKeywordKind(identifier));

			return needsEscaping ? "@" + identifier : identifier;
		}

		public static SyntaxToken ToIdentifierToken(
			this string identifier,
			bool isQueryContext = false)
		{
			var escaped = identifier.EscapeIdentifier(isQueryContext);

			if (escaped.Length == 0 || escaped[0] != '@') return SyntaxFactory.Identifier(escaped);

			var unescaped = identifier.StartsWith("@", StringComparison.Ordinal)
				? identifier.Substring(1)
				: identifier;

			var token = SyntaxFactory.Identifier(
				default, SyntaxKind.None, "@" + unescaped, unescaped, default);

			if (!identifier.StartsWith("@", StringComparison.Ordinal))
				token = token.WithAdditionalAnnotations(Simplifier.Annotation);

			return token;
		}

		public static IdentifierNameSyntax ToIdentifierName(this string identifier)
		{
			return SyntaxFactory.IdentifierName(identifier.ToIdentifierToken());
		}

		/// <summary>
		/// Removes the "Attribute" suffix from the end of this typeName, if present.
		/// </summary>
		public static string RemoveAttributeSuffix(this string typeName)
		{
			const string ATTRIBUTE_SUFFIX = "Attribute";

			return typeName.EndsWith(ATTRIBUTE_SUFFIX) ?
				typeName.Replace("Attribute", string.Empty) :
				typeName;
		}
	}
}
