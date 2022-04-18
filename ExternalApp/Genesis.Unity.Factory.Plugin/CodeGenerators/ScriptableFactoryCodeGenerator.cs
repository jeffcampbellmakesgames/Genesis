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

			var symbolFactoryData = data
				.OfType<SymbolFactoryData>()
				.ToArray();

			var codeGenFiles = new List<CodeGenFile>();
			codeGenFiles.AddRange(factoryKeyEnumData.Select(CreateFactoryEnumCodeGenFile));
			codeGenFiles.AddRange(factoryKeyData.Select(CreateFactoryCodeGenFile));
			codeGenFiles.AddRange(symbolFactoryData.Select(CreateSymbolFactoryCodeGenFile));
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

		public CodeGenFile CreateSymbolFactoryCodeGenFile(SymbolFactoryData data)
		{
			return new CodeGenFile(
				data.GetFilename(),
				data.ReplaceTemplateTokens(SYMBOL_FACTORY_TEMPLATE),
				GENERATOR_NAME);
		}

		private const string ENUM_TEMPLATE =
@"using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using System.Linq;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
#endif

namespace Genesis
{
	[CreateAssetMenu(fileName = ""New${TypeName}"", menuName = ""JCMG/Genesis/Factory/${TypeName}"")]
	public sealed partial class ${TypeName} : ScriptableObject
	{
		[Serializable]
		private class Mapping
		{
			#if ODIN_INSPECTOR && UNITY_EDITOR
			[FoldoutGroup(""@key"")]
			#endif
			#pragma warning disable 0649
			public ${KeyFullType} key;

			#if ODIN_INSPECTOR && UNITY_EDITOR
			[FoldoutGroup(""@key"")]
			#endif
			public ${ValueFullType} value;
			#pragma warning restore 0649
		}

		#if ODIN_INSPECTOR && UNITY_EDITOR
		[ValidateInput(nameof(EnsureAllKeyValuesAreUnique),
			""Not all Key values are unique, please ensure each one is unique."")]
		[ListDrawerSettings(
			Expanded = true,
			ShowIndexLabels = false,
			HideAddButton = true,
			OnTitleBarGUI = nameof(DrawTitleBarGUI))]
		#endif
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
		public bool TryGetValue(${KeyFullType} key, out ${ValueFullType} value)
		{
			value = null;

			Mapping mapping;
			if (!MappingLookup.TryGetValue(key, out mapping))
			{
				return false;
			}

			value = mapping.value;

			return value != null;
		}

		#if ODIN_INSPECTOR && UNITY_EDITOR
		private bool EnsureAllKeyValuesAreUnique(List<Mapping> values)
		{
			return values.GroupBy(x => x.key).All(x => x.Count() < 2);
		}

		[PropertyOrder(-1)]
		[Button]
		private void Sort()
		{
			_mappings.Sort(Comparison);

			#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
			#endif
		}

		private int Comparison(Mapping x, Mapping y)
		{
			var xSymbol = x != null ? x.key.ToString() : string.Empty;
			var ySymbol = y != null ? y.key.ToString() : string.Empty;

			return string.Compare(xSymbol, ySymbol, StringComparison.Ordinal);
		}

		private void DrawTitleBarGUI()
		{
			if (!ShouldShowAddButton())
			{
				return;
			}

			if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
			{
				TryAddMapping();
			}
		}

		private void TryAddMapping()
		{
			var enumValues = (${KeyFullType}[])Enum.GetValues(typeof(${KeyFullType}));
			foreach (var enumValue in enumValues)
			{
				if (_mappings.All(x => x.key != enumValue))
				{
					_mappings.Add(new Mapping()
					{
						key = enumValue
					});

					#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(this);
					#endif

					break;
				}
			}
		}

		private bool ShouldShowAddButton()
		{
			var enumValues = (${KeyFullType}[])Enum.GetValues(typeof(${KeyFullType}));
			return enumValues.Any(x => _mappings.All(y => y.key != x));
		}
		#endif
	}
}
";
		private const string GENERAL_TEMPLATE =
@"using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Genesis
{
	[CreateAssetMenu(fileName = ""New${TypeName}"", menuName = ""JCMG/Genesis/Factory/${TypeName}"")]
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

		#if ODIN_INSPECTOR
		[ListDrawerSettings(
			Expanded = true,
			ShowIndexLabels = false)]
		#endif
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
		public bool TryGetValue(${KeyFullType} key, out ${ValueFullType} value)
		{
			value = null;

			Mapping mapping;
			if (!MappingLookup.TryGetValue(key, out mapping))
			{
				return false;
			}

			value = mapping.value;

			return value != null;
		}
	}
}
";

		private const string SYMBOL_FACTORY_TEMPLATE =
@"using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using System.Linq;
using Sirenix.OdinInspector;
#endif

namespace Genesis
{
	[CreateAssetMenu(fileName = ""New${TypeName}"", menuName = ""JCMG/Genesis/Factory/${TypeName}"")]
	public sealed partial class ${TypeName} : ScriptableObject,
		IReadOnlyList<${ValueFullType}>
	{
		#if ODIN_INSPECTOR
		[ValidateInput(nameof(EnsureAllSymbolValuesAreUnique),
			""Not all Symbol values are unique, please ensure each one is unique."")]
		[ValidateInput(nameof(EnsureAllSymbolValuesAreNonNullOrEmpty),
			""At least one Symbol value is null or empty, please ensure none are null or empty."")]
		[ListDrawerSettings(
			Expanded = true,
			ShowIndexLabels = false)]
		#endif
		#pragma warning disable 0649
		[SerializeField]
		private List<${ValueFullType}> _mappings;
		#pragma warning restore 0649

		private Dictionary<string, ${ValueFullType}> MappingLookup
		{
			get
			{
				if(_mappingLookup == null)
				{
					_mappingLookup = new Dictionary<string, ${ValueFullType}>();
					for (var i = 0; i < _mappings.Count; i++)
					{
						if(_mappingLookup.ContainsKey(_mappings[i].Symbol))
						{
							continue;
						}

						_mappingLookup.Add(_mappings[i].Symbol, _mappings[i]);
					}
				}

				return _mappingLookup;
			}
		}

		private Dictionary<string, ${ValueFullType}> _mappingLookup;

		private void OnEnable()
		{
			if(_mappings == null)
			{
				_mappings = new List<${ValueFullType}>();
			}
		}

		/// <summary>
		/// Returns true if a mapping is found for <paramref name=""symbol""/> to a <see cref=""${ValueFullType}""/>,
		/// otherwise false.
		/// </summary>
		public bool TryGetValue(string symbol, out ${ValueFullType} value)
		{
			value = null;

			return MappingLookup.TryGetValue(symbol, out value);
		}

		#region IReadonlyList
		/// <inheritdoc />
		public IEnumerator<${ValueFullType}> GetEnumerator()
		{
			return _mappings.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public int Count => _mappings.Count;

		/// <inheritdoc />
		public ${ValueFullType} this[int index] => _mappings[index];
		#endregion

		#if ODIN_INSPECTOR
		private bool EnsureAllSymbolValuesAreUnique(List<${ValueFullType}> values)
		{
			return values.GroupBy(x => x != null
				? x.Symbol
				: string.Empty).All(x => x.Count() < 2);
		}

		private bool EnsureAllSymbolValuesAreNonNullOrEmpty(List<${ValueFullType}> values)
		{
			return values.All(x => x != null && !string.IsNullOrEmpty(x.Symbol));
		}

		[PropertyOrder(-1)]
		[Button]
		private void Sort()
		{
			_mappings.Sort(Comparison);

			#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
			#endif
		}

		private int Comparison(${ValueFullType} x, ${ValueFullType} y)
		{
			var xSymbol = x != null ? x.Symbol : string.Empty;
			var ySymbol = y != null ? y.Symbol : string.Empty;

			return string.Compare(xSymbol, ySymbol, StringComparison.Ordinal);

		}
		#endif
	}
}
";
	}
}
