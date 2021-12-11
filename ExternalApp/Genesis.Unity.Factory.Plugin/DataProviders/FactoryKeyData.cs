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
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Genesis.Unity.Factory.Plugin
{
	internal sealed class FactoryKeyData : CodeGeneratorData
	{
		// Keys
		private const string KEY_TYPE = "Factory.KeyType";
		private const string VALUE_TYPE = "Factory.ValueType";

		// Format
		private const string OUTPUT_FILE_PATH_FORMAT = @"Factory\{0}.cs";
		private const string FINAL_TYPE_NAME_FORMAT = @"{0}To{1}Factory";

		// Template Tokens
		private const string TYPE_NAME_TOKEN = "${TypeName}";
		private const string KEY_FULL_TYPE_TOKEN = "${KeyFullType}";
		private const string VALUE_FULL_TYPE_TOKEN = "${ValueFullType}";

		private const string EXCEPTION_KEY = "No type found for Key.";
		private const string EXCEPTION_VALUE = "No type found for Value.";

		public FactoryKeyData(ITypeSymbol keySymbol, ITypeSymbol valueSymbol)
		{
			this[KEY_TYPE] = keySymbol;
			this[VALUE_TYPE] = valueSymbol;
		}

		public string GetKeyHumanReadableName()
		{
			return GetKeyNamedTypeSymbol().GetHumanReadableName();
		}

		public string GetKeyFullTypeName()
		{
			return GetKeyNamedTypeSymbol().ToString();
		}

		public bool IsKeyTypeAnArray()
		{
			return GetKeyNamedTypeSymbol().IsArrayType();
		}

		public bool IsKeyTypGeneric()
		{
			return GetKeyNamedTypeSymbol().IsGenericType();
		}

		public ITypeSymbol GetKeyNamedTypeSymbol()
		{
			if (TryGetValue(KEY_TYPE, out var result))
			{
				return (ITypeSymbol)result;
			}

			throw new Exception(EXCEPTION_KEY);
		}

		public string GetValueHumanReadableName()
		{
			return GetValueNamedTypeSymbol().GetHumanReadableName();
		}

		public string GetValueFullTypeName()
		{
			return GetValueNamedTypeSymbol().ToString();
		}

		public ITypeSymbol GetValueNamedTypeSymbol()
		{
			if (TryGetValue(VALUE_TYPE, out var result))
			{
				return (ITypeSymbol)result;
			}

			throw new Exception(EXCEPTION_VALUE);
		}

		public bool IsValueTypeAnArray()
		{
			return GetValueNamedTypeSymbol().IsArrayType();
		}

		public bool IsValueTypGeneric()
		{
			return GetValueNamedTypeSymbol().IsGenericType();
		}

		public string GetFilename()
		{
			return string.Format(OUTPUT_FILE_PATH_FORMAT, GetTypeName());
		}

		public string ReplaceTemplateTokens(string template)
		{
			return template
				.Replace(TYPE_NAME_TOKEN, GetTypeName())
				.Replace(KEY_FULL_TYPE_TOKEN, GetKeyFullTypeName())
				.Replace(VALUE_FULL_TYPE_TOKEN, GetValueFullTypeName());
		}

		private string GetTypeName()
		{
			return string.Format(FINAL_TYPE_NAME_FORMAT, GetKeyHumanReadableName(), GetValueHumanReadableName());
		}
	}
}
