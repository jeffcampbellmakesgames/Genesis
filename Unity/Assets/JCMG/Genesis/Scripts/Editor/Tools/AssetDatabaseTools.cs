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

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Helper methods for the AssetDatabase
	/// </summary>
	public static class AssetDatabaseTools
	{
		private static readonly Type SCRIPTABLE_OBJECT_TYPE;
		private static readonly Dictionary<Type, ScriptableObject> SCRIPTABLE_ASSET_LOOKUP;

		private const string FILTER_TYPE_SEARCH = "t:{0}";

		static AssetDatabaseTools()
		{
			SCRIPTABLE_OBJECT_TYPE = typeof(ScriptableObject);
			SCRIPTABLE_ASSET_LOOKUP = new Dictionary<Type, ScriptableObject>();
		}
		/// <summary>
		/// Returns the first <see cref="ScriptableObject"/> asset found of Type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="asset"></param>
		/// <returns></returns>
		public static bool TryGetSingleScriptableAsset<T>(out T asset)
			where T : ScriptableObject
		{
			asset = null;

			var type = typeof(T);

			if (SCRIPTABLE_ASSET_LOOKUP.ContainsKey(type))
			{
				asset = (T)SCRIPTABLE_ASSET_LOOKUP[type];
			}
			else
			{
				var assetGUIDs = AssetDatabase.FindAssets(string.Format(FILTER_TYPE_SEARCH, type.Name));
				if (assetGUIDs.Length > 0)
				{
					var firstAssetGUID = assetGUIDs[0];
					var firstAssetPath = AssetDatabase.GUIDToAssetPath(firstAssetGUID);
					asset = (T)AssetDatabase.LoadAssetAtPath(firstAssetPath, SCRIPTABLE_OBJECT_TYPE);

					if (asset != null)
					{
						SCRIPTABLE_ASSET_LOOKUP.Add(type, asset);
					}
				}
			}

			return asset != null;
		}

		/// <summary>
		/// Returns all assets of type <typeparamref name="T"/> found in the Project.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T[] GetAssets<T>()
			where T : UnityEngine.Object
		{
			var assets = new List<T>();
			var assetGUIDs = AssetDatabase.FindAssets(string.Format(FILTER_TYPE_SEARCH, typeof(T).Name));
			for (var i = 0; i < assetGUIDs.Length; i++)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
				var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
				if (asset != null)
				{
					assets.Add(asset);
				}
			}

			return assets.ToArray();
		}
	}
}
