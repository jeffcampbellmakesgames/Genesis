using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using System.Linq;
using Sirenix.OdinInspector;
#endif

namespace Genesis
{
	[CreateAssetMenu(fileName = "DefaultPOCOSymbolObjectSymbolFactory", menuName = "Genesis/Factory/POCOSymbolObjectSymbolFactory")]
	public sealed partial class POCOSymbolObjectSymbolFactory : ScriptableObject
	{
		#if ODIN_INSPECTOR
		[ValidateInput(nameof(EnsureAllSymbolValuesAreUnique),
			"Not all Symbol values are unique, please ensure each one is unique.")]
		[ValidateInput(nameof(EnsureAllSymbolValuesAreNonNullOrEmpty),
			"At least one Symbol value is null or empty, please ensure none are null or empty.")]
		[ListDrawerSettings(
			Expanded = true,
			ShowIndexLabels = false
			)]
		#endif
		#pragma warning disable 0649
		[SerializeField]
		private List<ExampleContent.POCOSymbolObject> _mappings;
		#pragma warning restore 0649

		private Dictionary<string, ExampleContent.POCOSymbolObject> MappingLookup
		{
			get
			{
				if(_mappingLookup == null)
				{
					_mappingLookup = new Dictionary<string, ExampleContent.POCOSymbolObject>();
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

		private Dictionary<string, ExampleContent.POCOSymbolObject> _mappingLookup;

		private void OnEnable()
		{
			if(_mappings == null)
			{
				_mappings = new List<ExampleContent.POCOSymbolObject>();
			}
		}

		/// <summary>
		/// Returns true if a mapping is found for <paramref name="symbol"/> to a <see cref="ExampleContent.POCOSymbolObject"/>,
		/// otherwise false.
		/// </summary>
		public bool TryGetValue(string symbol, out ExampleContent.POCOSymbolObject value)
		{
			value = null;

			return MappingLookup.TryGetValue(symbol, out var mapping);
		}

		#if ODIN_INSPECTOR
		private bool EnsureAllSymbolValuesAreUnique(List<ExampleContent.POCOSymbolObject> values)
		{
			return values.GroupBy(x => x != null
				? x.Symbol
				: string.Empty).All(x => x.Count() < 2);
		}

		private bool EnsureAllSymbolValuesAreNonNullOrEmpty(List<ExampleContent.POCOSymbolObject> values)
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

		private int Comparison(ExampleContent.POCOSymbolObject x, ExampleContent.POCOSymbolObject y)
		{
			var xSymbol = x != null ? x.Symbol : string.Empty;
			var ySymbol = y != null ? y.Symbol : string.Empty;

			return string.Compare(xSymbol, ySymbol, StringComparison.Ordinal);

		}
		#endif
	}
}
