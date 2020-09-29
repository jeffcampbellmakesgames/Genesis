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

using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Menu items for this library
	/// </summary>
	internal static class MenuItems
	{
		// Menu item paths
		private const string GENERATE_CODE_MENU_ITEM = "Tools/JCMG/Genesis/Generate CLI Code #%g";
		private const string BUG_OR_FEATURE_REQUEST_MENU_ITEM = "Tools/JCMG/Genesis/Submit bug or feature request";
		private const string CHECK_FOR_UPDATES_MENU_ITEM = "Tools/JCMG/Genesis/Check for Updates...";
		private const string DOCUMENTATION_MENU_ITEM = "Tools/JCMG/Genesis/Documentation...";
		private const string DONATE_MENU_ITEM = "Tools/JCMG/Genesis/Donate to support development";
		private const string ABOUT_MENU_ITEM = "Tools/JCMG/Genesis/About";

		// Context menu item paths
		private const string GENESIS_SETTINGS_GENERATE_CODE_MENU_ITEM = "Assets/Genesis/Generate Code";

		// Menu item priorities
		private const int GENERATE_CODE_PRIORITY = 1;
		private const int BUG_OR_FEATURE_REQUEST_PRIORITY = 100;
		private const int CHECK_FOR_UPDATES_PRIORITY = 101;
		private const int DOCUMENTATION_PRIORITY = 102;
		private const int DONATE_PRIORITY = 103;
		private const int ABOUT_PRIORITY = 200;

		// GitHub URLs
		private const string GITHUB_URL = "https://github.com/jeffcampbellmakesgames/genesis";
		private const string GITHUB_ISSUES_URL = "https://github.com/jeffcampbellmakesgames/genesis/issues";

		private const string GITHUB_API_LATEST_RELEASE =
			"https://api.github.com/repos/jeffcampbellmakesgames/Genesis/releases/latest";

		// KOFI URL
		private const string KOFI_URL = "https://ko-fi.com/stampyturtle";

		#region Window Menu Items

		[MenuItem(GENERATE_CODE_MENU_ITEM, priority = GENERATE_CODE_PRIORITY)]
		public static void RunCodeGeneration()
		{
			GenesisCLIRunner.RunCodeGeneration();
		}

		[MenuItem(BUG_OR_FEATURE_REQUEST_MENU_ITEM, priority = BUG_OR_FEATURE_REQUEST_PRIORITY)]
		internal static void OpenURLToGitHubIssuesSection()
		{
			Application.OpenURL(GITHUB_ISSUES_URL);
		}

		[MenuItem(CHECK_FOR_UPDATES_MENU_ITEM, false, CHECK_FOR_UPDATES_PRIORITY)]
		public static void DisplayUpdates()
		{
			SDKUpdateTools.CheckUpdateInfo(EditorConstants.SDK_NAME, GITHUB_API_LATEST_RELEASE);
		}

		[MenuItem(DOCUMENTATION_MENU_ITEM, false, DOCUMENTATION_PRIORITY)]
		public static void OpenURLToDocumentation()
		{
			Application.OpenURL(GITHUB_URL);
		}

		[MenuItem(DONATE_MENU_ITEM, priority = DONATE_PRIORITY)]
		internal static void OpenURLToKoFi()
		{
			Application.OpenURL(KOFI_URL);
		}

		[MenuItem(ABOUT_MENU_ITEM, priority = ABOUT_PRIORITY)]
		internal static void OpenAboutModalDialog()
		{
			AboutWindow.View();
		}

		#endregion

		#region Context Menu Items

		[MenuItem(GENESIS_SETTINGS_GENERATE_CODE_MENU_ITEM)]
		public static void InitiateSingleGenesisRun()
		{
			Assert.IsNotNull(Selection.assetGUIDs);
			Assert.IsTrue(Selection.assetGUIDs.Length >= 1);

			var settingsData = Selection.assetGUIDs.Select(
					assetGUID =>
					{
						var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
						return AssetDatabase.LoadAssetAtPath<GenesisSettings>(assetPath);
					})
				.ToArray();

			// TODO Rewrite to use external CLI
			//UnityCodeGenerator.GenerateMultiple(settingsData);
		}

		[MenuItem(GENESIS_SETTINGS_GENERATE_CODE_MENU_ITEM, isValidateFunction:true)]
		public static bool ValidateInitiateSingleGenesisRun()
		{
			if (Selection.assetGUIDs.Length == 0)
			{
				return false;
			}

			foreach (var assetGUID in Selection.assetGUIDs)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
				var settings = AssetDatabase.LoadAssetAtPath<GenesisSettings>(assetPath);
				if (settings == null)
				{
					return false;
				}
			}

			return true;
		}

		#endregion
	}
}
