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
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Contains a serialized settings lookup for Genesis Code Gen
	/// </summary>
	[CreateAssetMenu(fileName = "NewGenesisSettings", menuName = "Genesis/GenesisSettings")]
	public sealed class GenesisSettings : ScriptableObject
	{
		/// <summary>
		/// A serializable KeyValuePair for configuration.
		/// </summary>
		[Serializable]
		private class KVP
		{
			public string key;
			public string value;
		}

		private List<KVP> KeyValuePairs
		{
			get
			{
				if (_keyValuePairs == null)
				{
					_keyValuePairs = new List<KVP>();
				}

				return _keyValuePairs;
			}
			set { _keyValuePairs = value; }
		}

		[SerializeField]
		#pragma warning disable 649
		private List<KVP> _keyValuePairs;
		#pragma warning restore 649

		/// <summary>
		/// Gets the value for existing KeyValuePair with <paramref name="key"/>. If none is found a new KeyValuePair
		/// instance is created using the <paramref name="defaultValue"/> and returns it.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public string GetOrSetValue(string key, string defaultValue = "")
		{
			Assert.IsFalse(string.IsNullOrEmpty(key));

			for (var i = 0; i < KeyValuePairs.Count; i++)
			{
				if (KeyValuePairs[i].key == key)
				{
					return KeyValuePairs[i].value;
				}
			}

			KeyValuePairs.Add(new KVP
			{
				key = key,
				value = defaultValue
			});

			EditorUtility.SetDirty(this);

			return defaultValue;
		}

		/// <summary>
		/// Creates or updates an existing KeyValuePair.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetValue(string key, string value)
		{
			var foundValue = false;
			for (var i = 0; i < KeyValuePairs.Count; i++)
			{
				if (KeyValuePairs[i].key == key)
				{
					KeyValuePairs[i].value = value;
					foundValue = true;
					break;
				}
			}

			if (!foundValue)
			{
				KeyValuePairs.Add(
					new KVP
					{
						key = key, value = value
					});
			}

			EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// Creates an existing KeyValuePair if one is not already present.
		/// </summary>
		/// <param name="defaultValues"></param>
		public void SetIfNotPresent(Dictionary<string, string> defaultValues)
		{
			foreach (var kvp in defaultValues)
			{
				SetIfNotPresent(kvp.Key, kvp.Value);
			}
		}

		/// <summary>
		/// Creates an existing KeyValuePair if one is not already present.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetIfNotPresent(string key, string value)
		{
			for (var i = 0; i < KeyValuePairs.Count; i++)
			{
				if (KeyValuePairs[i].key == key)
				{
					return;
				}
			}

			KeyValuePairs.Add(
				new KVP
				{
					key = key, value = value
				});

			EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// Returns all <see cref="GenesisSettings"/> found in the Project.
		/// </summary>
		/// <returns></returns>
		internal static GenesisSettings[] GetAllSettings()
		{
			return AssetDatabaseTools.GetAssets<GenesisSettings>();
		}
	}
}
