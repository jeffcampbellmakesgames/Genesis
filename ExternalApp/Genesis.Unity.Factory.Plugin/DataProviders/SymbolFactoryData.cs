using System;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Genesis.Unity.Factory.Plugin
{
	/// <summary>
	/// Code-generation data for a factory for a symbol object.
	/// </summary>
	public sealed class SymbolFactoryData : CodeGeneratorData
	{
		// Keys
		private const string VALUE_TYPE = "Factory.ValueType";

		// Format
		private const string OUTPUT_FILE_PATH_FORMAT = @"Factory\{0}.cs";
		private const string FINAL_TYPE_NAME_FORMAT = @"{0}SymbolFactory";

		// Template Tokens
		private const string TYPE_NAME_TOKEN = "${TypeName}";
		private const string VALUE_FULL_TYPE_TOKEN = "${ValueFullType}";

		// Exceptions
		private const string EXCEPTION_VALUE = "No type found for Value.";

		public SymbolFactoryData(ITypeSymbol valueSymbol)
		{
			this[VALUE_TYPE] = valueSymbol;
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

		public string GetFilename()
		{
			return string.Format(OUTPUT_FILE_PATH_FORMAT, GetTypeName());
		}

		public string ReplaceTemplateTokens(string template)
		{
			return template
				.Replace(TYPE_NAME_TOKEN, GetTypeName())
				.Replace(VALUE_FULL_TYPE_TOKEN, GetValueFullTypeName());
		}

		private string GetTypeName()
		{
			return string.Format(FINAL_TYPE_NAME_FORMAT, GetValueHumanReadableName());
		}
	}
}
