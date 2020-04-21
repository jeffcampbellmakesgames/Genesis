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

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// An editor class for managing project and user preferences for the Genesis library.
	/// </summary>
	internal static class GenesisPreferences
	{
		/// <summary>
		/// Returns the preferred <see cref="LogLevel"/>.
		/// </summary>
		public static LogLevel PreferredLogLevel
		{
			get
			{
				if (!_logLevel.HasValue)
				{
					_logLevel = (LogLevel)GetIntPref(LOG_LEVEL_PREF, (int)DEFAULT_LOG_LEVEL);
				}

				return _logLevel.Value;
			}
			set
			{
				_logLevel = value;
				EditorPrefs.SetInt(LOG_LEVEL_PREF, (int)value);
			}
		}

		/// <summary>
		/// Returns true if the code generation should execute as a dry run, otherwise false.
		/// </summary>
		public static bool ExecuteDryRun
		{
			get
			{
				if (!_executeDryRun.HasValue)
				{
					_executeDryRun = GetBoolPref(ENABLE_DRY_RUN_PREF, ENABLE_DRY_RUN_DEFAULT);
				}

				return _executeDryRun.Value;
			}
			set
			{
				_executeDryRun = value;
				EditorPrefs.SetBool(ENABLE_DRY_RUN_PREF, value);
			}
		}


		// UI
		private const string PREFERENCES_TITLE_PATH = "Preferences/Genesis";
		private const string PROJECT_TITLE_PATH = "Project/Genesis";

		private const string USER_PREFERENCES_HEADER = "User Preferences";
		private const string PROJECT_REFERENCES_HEADER = "Project Preferences";

		// Config UI
		private const string LOG_LEVEL_LABEL = "Logging Level";
		private const string ENABLE_DRY_RUN_LABEL = "Enable Dry Run";

		private const string LOG_LEVEL_DESCRIPTION
			= "The level of logs that should be output to the Unity Console.";

		private const string ENABLE_DRY_RUN_DESCRIPTION
			= "If enabled, the code generation process when executed will not write any generated code to disk.";

		// Searchable Fields
		private static readonly string[] KEYWORDS =
		{
			"JCMG",
			"genesis",
			"code generation",
			"code gen",
			"code",
			"generation",
			"gen"
		};

		private static readonly IPreferencesDrawer[] PREFERENCES_DRAWERS;

		// User Editor Preferences
		private const string LOG_LEVEL_PREF = "Genesis.LogLevel";
		private const string ENABLE_DRY_RUN_PREF = "Genesis.DryRun";

		private const LogLevel DEFAULT_LOG_LEVEL = LogLevel.Info;
		private const bool ENABLE_DRY_RUN_DEFAULT = false;

		private static Vector2 _scrollViewPosition;

		// Cacheable Prefs
		private static bool? _executeDryRun;
		private static LogLevel? _logLevel;

		static GenesisPreferences()
		{
			PREFERENCES_DRAWERS = ReflectionTools.GetAllImplementingInstancesOfInterface<IPreferencesDrawer>().ToArray();

			var settings = GenesisSettings.GetOrCreateSettings();

			foreach (var preferencesDrawer in PREFERENCES_DRAWERS)
			{
				preferencesDrawer.Initialize(settings);
			}

			EditorUtility.SetDirty(settings);
		}

		[SettingsProvider]
		private static SettingsProvider CreateProjectPreferenceSettingsProvider()
		{
			return new SettingsProvider(PROJECT_TITLE_PATH, SettingsScope.Project)
			{
				guiHandler = DrawProjectGUI,
				keywords = KEYWORDS
			};
		}

		[SettingsProvider]
		private static SettingsProvider CreatePersonalPreferenceSettingsProvider()
		{
			return new SettingsProvider(PREFERENCES_TITLE_PATH, SettingsScope.User)
			{
				guiHandler = DrawPersonalPrefsGUI,
				keywords = KEYWORDS
			};
		}

		private static void DrawProjectGUI(string value = "")
		{
			EditorGUILayout.LabelField(PROJECT_REFERENCES_HEADER, EditorStyles.boldLabel);

			_scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
			using (var scope = new EditorGUI.ChangeCheckScope())
			{
				var settings = GenesisSettings.GetOrCreateSettings();
				for (var i = 0; i < PREFERENCES_DRAWERS.Length; i++)
				{
					var preferencesDrawer = PREFERENCES_DRAWERS[i];
					preferencesDrawer.DrawHeader(settings);
					preferencesDrawer.DrawContent(settings);

					if (i < PREFERENCES_DRAWERS.Length - 1)
					{
						EditorGUILayout.Space();
					}
				}

				if (scope.changed)
				{
					EditorUtility.SetDirty(settings);
				}
			}

			EditorGUILayout.EndScrollView();
		}

		private static void DrawPersonalPrefsGUI(string value = "")
		{
			EditorGUILayout.LabelField(USER_PREFERENCES_HEADER, EditorStyles.boldLabel);

			GUI.changed = false;

			// Log Level
			EditorGUILayout.HelpBox(LOG_LEVEL_DESCRIPTION, MessageType.Info);
			using (var scope = new EditorGUI.ChangeCheckScope())
			{
				var newValue = (LogLevel)EditorGUILayout.EnumPopup(LOG_LEVEL_LABEL, PreferredLogLevel);

				if (scope.changed)
				{
					PreferredLogLevel = newValue;
				}
			}

			// Enable Dry Run
			EditorGUILayout.HelpBox(ENABLE_DRY_RUN_DESCRIPTION, MessageType.Info);
			using(var scope = new EditorGUI.ChangeCheckScope())
			{
				var newValue = EditorGUILayout.Toggle(ENABLE_DRY_RUN_LABEL, ExecuteDryRun);

				if(scope.changed)
				{
					ExecuteDryRun = newValue;
				}
			}
		}

		/// <summary>
		/// Returns the current bool preference; if none exists, the default is set and returned.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private static bool GetBoolPref(string key, bool defaultValue)
		{
			if (!EditorPrefs.HasKey(key))
			{
				EditorPrefs.SetBool(key, defaultValue);
			}

			return EditorPrefs.GetBool(key);
		}

		/// <summary>
		/// Returns the current int preference; if none exists, the default is set and returned.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private static int GetIntPref(string key, int defaultValue)
		{
			if (!EditorPrefs.HasKey(key))
			{
				EditorPrefs.SetInt(key, defaultValue);
			}

			return EditorPrefs.GetInt(key);
		}
	}
}
