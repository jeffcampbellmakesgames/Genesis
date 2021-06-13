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
using Genesis.Shared;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Contains a serialized settings lookup for Genesis Code Generation.
	/// </summary>
	[CreateAssetMenu(
		fileName = "NewGenesisSettings",
		menuName = EditorConstants.CREATE_ASSET_MENU_ITEM_PREFIX + "GenesisSettings")]
	public sealed class GenesisSettings : ScriptableObject,
										  IGenesisConfig
	{
		#pragma warning disable 0612
		#pragma warning disable 0649

		/// <summary>
		/// A serializable KeyValuePair for configuration.
		/// </summary>
		[Obsolete]
		[Serializable]
		private class KVP
		{
			public string key;
			public string value;
		}

		[Obsolete]
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
			set => _keyValuePairs = value;
		}

		[HideInInspector]
		[SerializeField]
		private List<KVP> _keyValuePairs;

		[SerializeField]
		private string _genesisCLIInstallationFolder;

		[SerializeField]
		private GenesisConfig _genesisConfig;
		#pragma warning restore 0649

		private void OnValidate()
		{
			// Migrate the old key-value pairs from a local list to a GenesisConfig
			if (_genesisConfig == null)
			{
				_genesisConfig = new GenesisConfig();

				for (var i = 0; i < _keyValuePairs.Count; i++)
				{
					_genesisConfig.SetValue(_keyValuePairs[i].key, _keyValuePairs[i].value);
				}

				EditorUtility.SetDirty(this);
			}

			// Always set the genesis config name to the name of this ScriptableObject asset.
			if (_genesisConfig.name != name)
			{
				_genesisConfig.name = name;

				EditorUtility.SetDirty(this);
			}
		}

		#pragma warning restore 0612

		#region Command-Line

		/// <summary>
		/// Returns the Json version of this config instance.
		/// </summary>
		public string ConvertToJson()
		{
			return JsonUtility.ToJson(_genesisConfig);
		}

		/// <summary>
		/// Returns the Json version of this config instance.
		/// </summary>
		public void ConvertFromJson(string json)
		{
			_genesisConfig = JsonUtility.FromJson<GenesisConfig>(json);

			EditorUtility.SetDirty(this);
		}

		#endregion

		#region IGenesisConfig

		/// <summary>
		/// The human-readable name of this configuration.
		/// </summary>
		public string Name => _genesisConfig.name;

		/// <summary>
		/// Returns true if an existing KeyValuePair exists for <paramref name="key"/>, otherwise false.
		/// </summary>
		public bool HasValue(string key)
		{
			return _genesisConfig.HasValue(key);
		}

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

			var value = _genesisConfig.GetOrSetValue(key, defaultValue);

			EditorUtility.SetDirty(this);

			return value;
		}

		/// <summary>
		/// Creates or updates an existing KeyValuePair.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetValue(string key, string value)
		{
			_genesisConfig.SetValue(key, value);

			EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// Creates an existing KeyValuePair if one is not already present.
		/// </summary>
		/// <param name="defaultValues"></param>
		public void SetIfNotPresent(Dictionary<string, string> defaultValues)
		{
			_genesisConfig.SetIfNotPresent(defaultValues);
		}

		/// <summary>
		/// Creates an existing KeyValuePair if one is not already present.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetIfNotPresent(string key, string value)
		{
			_genesisConfig.SetIfNotPresent(key, value);

			EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/> and configures it with <see cref="KeyValuePair"/>
		/// instances from this <see cref="IGenesisConfig"/>.
		/// </summary>
		public T CreateAndConfigure<T>()
			where T : IConfigurable, new()
		{
			return _genesisConfig.CreateAndConfigure<T>();
		}

		#endregion

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
