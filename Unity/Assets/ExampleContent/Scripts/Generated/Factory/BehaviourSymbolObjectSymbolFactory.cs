using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using System.Linq;
using Sirenix.OdinInspector;
#endif

namespace Genesis
{
	[CreateAssetMenu(fileName = "NewBehaviourSymbolObjectSymbolFactory", menuName = "JCMG/Genesis/Factory/BehaviourSymbolObjectSymbolFactory")]
	public sealed partial class BehaviourSymbolObjectSymbolFactory : ScriptableObject,
		IReadOnlyList<ExampleContent.BehaviourSymbolObject>
	{
		#if ODIN_INSPECTOR
		[ValidateInput(nameof(EnsureAllSymbolValuesAreUnique),
			"Not all Symbol values are unique, please ensure each one is unique.")]
		[ValidateInput(nameof(EnsureAllSymbolValuesAreNonNullOrEmpty),
			"At least one Symbol value is null or empty, please ensure none are null or empty.")]
		[ListDrawerSettings(
			Expanded = true,
			ShowIndexLabels = false)]
		#endif
		#pragma warning disable 0649
		[SerializeField]
		private List<ExampleContent.BehaviourSymbolObject> _mappings;
		#pragma warning restore 0649

		private Dictionary<string, ExampleContent.BehaviourSymbolObject> MappingLookup
		{
			get
			{
				if(_mappingLookup == null)
				{
					_mappingLookup = new Dictionary<string, ExampleContent.BehaviourSymbolObject>();
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

		private Dictionary<string, ExampleContent.BehaviourSymbolObject> _mappingLookup;

		private void OnEnable()
		{
			if(_mappings == null)
			{
				_mappings = new List<ExampleContent.BehaviourSymbolObject>();
			}
		}

		/// <summary>
		/// Returns true if a mapping is found for <paramref name="symbol"/> to a <see cref="ExampleContent.BehaviourSymbolObject"/>,
		/// otherwise false.
		/// </summary>
		public bool TryGetValue(string symbol, out ExampleContent.BehaviourSymbolObject value)
		{
			value = null;

			return MappingLookup.TryGetValue(symbol, out value);
		}

		#region IReadonlyList
		/// <inheritdoc />
		public IEnumerator<ExampleContent.BehaviourSymbolObject> GetEnumerator()
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
		public ExampleContent.BehaviourSymbolObject this[int index] => _mappings[index];
		#endregion

		#if ODIN_INSPECTOR
		private bool EnsureAllSymbolValuesAreUnique(List<ExampleContent.BehaviourSymbolObject> values)
		{
			return values.GroupBy(x => x != null
				? x.Symbol
				: string.Empty).All(x => x.Count() < 2);
		}

		private bool EnsureAllSymbolValuesAreNonNullOrEmpty(List<ExampleContent.BehaviourSymbolObject> values)
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

		private int Comparison(ExampleContent.BehaviourSymbolObject x, ExampleContent.BehaviourSymbolObject y)
		{
			var xSymbol = x != null ? x.Symbol : string.Empty;
			var ySymbol = y != null ? y.Symbol : string.Empty;

			return string.Compare(xSymbol, ySymbol, StringComparison.Ordinal);

		}
		#endif
	}
}
