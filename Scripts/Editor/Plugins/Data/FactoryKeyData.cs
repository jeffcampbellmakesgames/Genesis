using System;

namespace JCMG.Genesis.Editor.Plugins
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

		public FactoryKeyData(Type keyType, Type valueType)
		{
			this[KEY_TYPE] = keyType;
			this[VALUE_TYPE] = valueType;
		}

		public Type GetKeyType()
		{
			if (TryGetValue(KEY_TYPE, out var result))
			{
				return (Type)result;
			}

			throw new Exception(EXCEPTION_KEY);
		}

		public string GetKeyShortTypeName()
		{
			return GetKeyType().GetHumanReadableName();
		}

		public string GetKeyFullTypeName()
		{
			return GetKeyType().GetFullTypeName();
		}

		public Type GetValueType()
		{
			if (TryGetValue(VALUE_TYPE, out var result))
			{
				return (Type)result;
			}

			throw new Exception(EXCEPTION_VALUE);
		}

		public string GetValueShortTypeName()
		{
			return GetValueType().GetHumanReadableName();
		}

		public string GetValueFullTypeName()
		{
			return GetValueType().GetFullTypeName();
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
			return string.Format(FINAL_TYPE_NAME_FORMAT, GetKeyShortTypeName(), GetValueShortTypeName());
		}
	}
}
