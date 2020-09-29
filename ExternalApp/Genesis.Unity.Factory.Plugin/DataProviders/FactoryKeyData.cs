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
		private const string FINAL_TYPE_NAME_FORMAT = @"{0}To{1}";

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
