using System.Collections.Generic;
using System.Linq;
using Genesis.Plugin;

namespace Genesis.Unity.Factory.Plugin
{
	internal sealed class ScriptableFactoryCodeGenerator : ICodeGenerator
	{
		/// <summary>
		/// The name of the plugin.
		/// </summary>
		public string Name => NAME;

		/// <summary>
		/// The priority value this plugin should be given to execute with regards to other plugins,
		/// ordered by ASC value.
		/// </summary>
		public int Priority => 0;

		/// <summary>
		/// Returns true if this plugin should be executed in Dry Run Mode, otherwise false.
		/// </summary>
		public bool RunInDryMode => true;

		private const string NAME = "Scriptable Factory Lookup";
		private static readonly string GENERATOR_NAME;

		static ScriptableFactoryCodeGenerator()
		{
			GENERATOR_NAME = nameof(ScriptableFactoryCodeGenerator);
		}

		public CodeGenFile[] Generate(CodeGeneratorData[] data)
		{
			var factoryKeyEnumData = data
				.OfType<FactoryKeyEnumData>()
				.ToArray();

			var factoryKeyData = data
				.OfType<FactoryKeyData>()
				.ToArray();

			var codeGenFiles = new List<CodeGenFile>();
			codeGenFiles.AddRange(factoryKeyEnumData.Select(CreateFactoryEnumCodeGenFile));
			codeGenFiles.AddRange(factoryKeyData.Select(CreateFactoryCodeGenFile));

			return codeGenFiles.ToArray();
		}

		private CodeGenFile CreateFactoryCodeGenFile(FactoryKeyData data)
		{
			return new CodeGenFile(
				data.GetFilename(),
				data.ReplaceTemplateTokens(GENERAL_TEMPLATE),
				GENERATOR_NAME);
		}

		public CodeGenFile CreateFactoryEnumCodeGenFile(FactoryKeyEnumData data)
		{
			return new CodeGenFile(
				data.GetFilename(),
				data.ReplaceTemplateTokens(ENUM_TEMPLATE),
				GENERATOR_NAME);
		}

		private const string ENUM_TEMPLATE
= @"
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Genesis
{
	[CreateAssetMenu(fileName = ""Default${TypeName}"", menuName = ""Genesis/Factory/${TypeName}"")]
	public sealed partial class ${TypeName} : ScriptableObject
	{
		[Serializable]
		private class Mapping
		{
			#pragma warning disable 0649
			public ${KeyFullType} key;

			public ${ValueFullType} value;
			#pragma warning restore 0649
		}

		#pragma warning disable 0649
		[SerializeField]
		private List<Mapping> _mappings;
		#pragma warning restore 0649

		private Dictionary<${KeyFullType}, Mapping> MappingLookup
		{
			get
			{
				if(_mappingLookup == null)
				{
					_mappingLookup = new Dictionary<${KeyFullType}, Mapping>();
					for (var i = 0; i < _mappings.Count; i++)
					{
						if(_mappingLookup.ContainsKey(_mappings[i].key))
						{
							continue;
						}

						_mappingLookup.Add(_mappings[i].key, _mappings[i]);
					}
				}

				return _mappingLookup;
			}
		}

		private Dictionary<${KeyFullType}, Mapping> _mappingLookup;

		private void OnEnable()
		{
			if(_mappings == null)
			{
				_mappings = new List<Mapping>();

				var values = (${KeyFullType}[])Enum.GetValues(typeof(${KeyFullType}));
				for (var i = 0; i < values.Length; i++)
				{
					_mappings.Add(new Mapping
					{
						key = values[i]
					});
				}
			}
		}

		/// <summary>
		/// Returns true if a mapping is found for <see cref=""${KeyFullType}""/> <paramref name=""key""/> to a
		/// <see cref=""${ValueFullType}""/>, otherwise false.
		/// </summary>
		/// <param name=""key""></param>
		/// <param name=""value""></param>
		/// <returns></returns>
		public bool TryGetValue(${KeyFullType} key, out ${ValueFullType} value)
		{
			value = null;

			Mapping mapping;
			if (!MappingLookup.TryGetValue(key, out mapping))
			{
				return false;
			}

			value = mapping.value;

			return true;
		}
	}
}
";
		private const string GENERAL_TEMPLATE
			= @"
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Genesis
{
	[CreateAssetMenu(fileName = ""Default${TypeName}"", menuName = ""Genesis/Factory/${TypeName}"")]
	public sealed partial class ${TypeName} : ScriptableObject
	{
		[Serializable]
		private class Mapping
		{
			#pragma warning disable 0649
			public ${KeyFullType} key;

			public ${ValueFullType} value;
			#pragma warning restore 0649
		}

		#pragma warning disable 0649
		[SerializeField]
		private List<Mapping> _mappings;
		#pragma warning restore 0649

		private Dictionary<${KeyFullType}, Mapping> MappingLookup
		{
			get
			{
				if(_mappingLookup == null)
				{
					_mappingLookup = new Dictionary<${KeyFullType}, Mapping>();
					for (var i = 0; i < _mappings.Count; i++)
					{
						if(_mappingLookup.ContainsKey(_mappings[i].key))
						{
							continue;
						}

						_mappingLookup.Add(_mappings[i].key, _mappings[i]);
					}
				}

				return _mappingLookup;
			}
		}

		private Dictionary<${KeyFullType}, Mapping> _mappingLookup;

		private void OnEnable()
		{
			if(_mappings == null)
			{
				_mappings = new List<Mapping>();
			}
		}

		/// <summary>
		/// Returns true if a mapping is found for <see cref=""${KeyFullType}""/> <paramref name=""key""/> to a
		/// <see cref=""${ValueFullType}""/>, otherwise false.
		/// </summary>
		/// <param name=""key""></param>
		/// <param name=""value""></param>
		/// <returns></returns>
		public bool TryGetValue(${KeyFullType} key, out ${ValueFullType} value)
		{
			value = null;

			Mapping mapping;
			if (!MappingLookup.TryGetValue(key, out mapping))
			{
				return false;
			}

			value = mapping.value;

			return true;
		}
	}
}
";
	}
}
