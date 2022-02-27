using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using System.Linq;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
#endif

namespace Genesis
{
	[CreateAssetMenu(fileName = "NewItemTypeToSpriteFactory", menuName = "JCMG/Genesis/Factory/ItemTypeToSpriteFactory")]
	public sealed partial class ItemTypeToSpriteFactory : ScriptableObject
	{
		[Serializable]
		private class Mapping
		{
			#if ODIN_INSPECTOR && UNITY_EDITOR
			[FoldoutGroup("@key")]
			#endif
			#pragma warning disable 0649
			public ExampleContent.ItemType key;

			#if ODIN_INSPECTOR && UNITY_EDITOR
			[FoldoutGroup("@key")]
			#endif
			public UnityEngine.Sprite value;
			#pragma warning restore 0649
		}

		#if ODIN_INSPECTOR && UNITY_EDITOR
		[ValidateInput(nameof(EnsureAllKeyValuesAreUnique),
			"Not all Key values are unique, please ensure each one is unique.")]
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

		private Dictionary<ExampleContent.ItemType, Mapping> MappingLookup
		{
			get
			{
				if(_mappingLookup == null)
				{
					_mappingLookup = new Dictionary<ExampleContent.ItemType, Mapping>();
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

		private Dictionary<ExampleContent.ItemType, Mapping> _mappingLookup;

		private void OnEnable()
		{
			if(_mappings == null)
			{
				_mappings = new List<Mapping>();

				var values = (ExampleContent.ItemType[])Enum.GetValues(typeof(ExampleContent.ItemType));
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
		/// Returns true if a mapping is found for <see cref="ExampleContent.ItemType"/> <paramref name="key"/> to a
		/// <see cref="UnityEngine.Sprite"/>, otherwise false.
		/// </summary>
		public bool TryGetValue(ExampleContent.ItemType key, out UnityEngine.Sprite value)
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
			var enumValues = (ExampleContent.ItemType[])Enum.GetValues(typeof(ExampleContent.ItemType));
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
			var enumValues = (ExampleContent.ItemType[])Enum.GetValues(typeof(ExampleContent.ItemType));
			return enumValues.Any(x => _mappings.All(y => y.key != x));
		}
		#endif
	}
}
