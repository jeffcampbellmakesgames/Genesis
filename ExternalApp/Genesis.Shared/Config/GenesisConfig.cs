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
using System.Diagnostics;

namespace Genesis.Shared
{
	/// <summary>
	/// A serializable config made up of key-value pairs.
	/// </summary>
	[Serializable]
	internal class GenesisConfig : IGenesisConfig
	{
		/// <summary>
		/// The human-readable name of this configuration.
		/// </summary>
		public string Name => name;

		/// <summary>
		/// All <see cref="KeyValuePair"/> instances defining this configuration.
		/// </summary>
		public List<KeyValuePair> keyValuePairs;

		/// <summary>
		/// The human-readable name of this configuration.
		/// </summary>
		public string name;

		public GenesisConfig()
		{
			keyValuePairs = new List<KeyValuePair>();
			name = "DefaultGenesisConfig";
		}

		/// <summary>
		/// Returns true if an existing KeyValuePair exists for <paramref name="key"/>, otherwise false.
		/// </summary>
		public bool HasValue(string key)
		{
			for (var i = 0; i < keyValuePairs.Count; i++)
			{
				if (keyValuePairs[i].key == key)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the value for existing KeyValuePair with <paramref name="key"/>. If none is found a new KeyValuePair
		/// instance is created using the <paramref name="defaultValue"/> and returns it.
		/// </summary>
		public string GetOrSetValue(string key, string defaultValue = "")
		{
			Debug.Assert(!string.IsNullOrEmpty(key));

			for (var i = 0; i < keyValuePairs.Count; i++)
			{
				if (keyValuePairs[i].key == key)
				{
					return keyValuePairs[i].value;
				}
			}

			keyValuePairs.Add(new KeyValuePair {key = key, value = defaultValue});

			return defaultValue;
		}

		/// <summary>
		/// Creates or updates an existing KeyValuePair.
		/// </summary>
		public void SetValue(string key, string value)
		{
			var foundValue = false;
			for (var i = 0; i < keyValuePairs.Count; i++)
			{
				if (keyValuePairs[i].key == key)
				{
					keyValuePairs[i].value = value;
					foundValue = true;
					break;
				}
			}

			if (!foundValue)
			{
				keyValuePairs.Add(new KeyValuePair {key = key, value = value});
			}
		}

		/// <summary>
		/// Creates an existing KeyValuePair if one is not already present.
		/// </summary>
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
		public void SetIfNotPresent(string key, string value)
		{
			for (var i = 0; i < keyValuePairs.Count; i++)
			{
				if (keyValuePairs[i].key == key)
				{
					return;
				}
			}

			keyValuePairs.Add(new KeyValuePair {key = key, value = value});
		}

		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/> and configures it with <see cref="KeyValuePair"/>
		/// instances from this <see cref="IGenesisConfig"/>.
		/// </summary>
		public T CreateAndConfigure<T>()
			where T : IConfigurable, new()
		{
			var configurable = new T();
			configurable.Configure(this);
			return configurable;
		}
	}
}
