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

using System.IO;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace JCMG.Genesis.Editor
{

	/// <summary>
	/// An editor class for managing project and user preferences for the Genesis library.
	/// </summary>
	internal static class GenesisPreferences
	{
		/// <summary>
		/// The installation folder for the Genesis command-line executable.
		/// </summary>
		public static string GenesisCLIInstallationFolder
		{
			get
			{
				if (_genesisCLIInstallationFolder == null)
				{
					_genesisCLIInstallationFolder = ProjectSettings.Get(
						GENESIS_INSTALLATION_PREF,
						SettingsScope.Project,
						string.Empty);
				}

				return _genesisCLIInstallationFolder;
			}
			set
			{
				_genesisCLIInstallationFolder = value;
				ProjectSettings.Set(GENESIS_INSTALLATION_PREF, value);
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
					_executeDryRun = EditorPrefs.GetBool(ENABLE_DRY_RUN_PREF, ENABLE_DRY_RUN_DEFAULT);
				}

				return _executeDryRun.Value;
			}
			set
			{
				_executeDryRun = value;
				EditorPrefs.SetBool(ENABLE_DRY_RUN_PREF, value);
			}
		}

		/// <summary>
		/// Returns true if the code generation should be executed with verbose logging, otherwise false.
		/// </summary>
		public static bool EnableVerboseLogging
		{
			get
			{
				if (!_enableVerboseLogging.HasValue)
				{
					_enableVerboseLogging = EditorPrefs.GetBool(ENABLE_VERBOSE_LOGGING_PREF, ENABLE_VERBOSE_LOGGING_DEFAULT);
				}

				return _enableVerboseLogging.Value;
			}
			set
			{
				_enableVerboseLogging = value;
				EditorPrefs.SetBool(ENABLE_VERBOSE_LOGGING_PREF, value);
			}
		}

		/// <summary>
		/// The project <see cref="Settings"/> instance for Genesis for this project.
		/// </summary>
		private static Settings ProjectSettings
		{
			get
			{
				if (_PROJECT_SETTINGS == null)
				{
					_PROJECT_SETTINGS = new Settings(PACKAGE_NAME, SETTINGS_NAME);
				}

				return _PROJECT_SETTINGS;
			}
		}

		private static Settings _PROJECT_SETTINGS;

		// Project Settings
		private const string PACKAGE_NAME = "com.jeffcampbellmakesgames.genesis";
		private const string SETTINGS_NAME = "GenesisSettings";

		// UI
		private const string PREFERENCES_TITLE_PATH = "Preferences/Genesis";
		private const string PROJECT_TITLE_PATH = "Project/Genesis";

		private const string USER_PREFERENCES_HEADER = "User Preferences";
		private const string PROJECT_REFERENCES_HEADER = "Project Preferences";

		// Labels
		private const string ENABLE_DRY_RUN_LABEL = "Enable Dry Run";
		private const string ENABLE_VERBOSE_LOGGING_LABEL = "Enable Verbose Logging";
		private const string GENESIS_CLI_INSTALL_FOLDER_LABEL = "Installation Folder";

		// Descriptions
		private const string ENABLE_DRY_RUN_DESCRIPTION
			= "If enabled, the code generation process when executed will not write any generated code to disk.";

		private const string ENABLE_VERBOSE_LOGGING_DESCRIPTION =
			"If enabled, additional information will be logged to the console.";

		private const string GENESIS_CLI_DIRECTORY_DESCRIPTION
			= "The location of the Genesis command-line executable and related files.";

		// Titles
		private const string GENESIS_CLI_FOLDER_SELECT_TITLE = "Select Genesis CLI Install Folder";

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

		// Project Editor Preferences
		private const string GENESIS_INSTALLATION_PREF = "Genesis.InstallationFolder";

		// User Editor Preferences
		private const string ENABLE_DRY_RUN_PREF = "Genesis.DryRun";
		private const string ENABLE_VERBOSE_LOGGING_PREF = "Genesis.IsVerbose";

		private const bool ENABLE_DRY_RUN_DEFAULT = false;
		private const bool ENABLE_VERBOSE_LOGGING_DEFAULT = true;

		// Cacheable Prefs
		private static bool? _executeDryRun;
		private static bool? _enableVerboseLogging;
		private static string _genesisCLIInstallationFolder;

		#region SettingsProvider and EditorGUI

		[SettingsProvider]
		private static SettingsProvider CreatePersonalPreferenceSettingsProvider()
		{
			return new SettingsProvider(PREFERENCES_TITLE_PATH, SettingsScope.User)
			{
				guiHandler = DrawPersonalPrefsGUI,
				keywords = KEYWORDS
			};
		}

		[SettingsProvider]
		private static SettingsProvider CreateSettingsProvider()
		{
			return new SettingsProvider(PROJECT_TITLE_PATH, SettingsScope.Project)
			{
				guiHandler = DrawProjectPrefsGUI,
				keywords = KEYWORDS
			};
		}

		private static void DrawPersonalPrefsGUI(string value = "")
		{
			EditorGUILayout.LabelField(USER_PREFERENCES_HEADER, EditorStyles.boldLabel);

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

			// Enable Verbose Logging
			EditorGUILayout.HelpBox(ENABLE_VERBOSE_LOGGING_DESCRIPTION, MessageType.Info);
			using (var scope = new EditorGUI.ChangeCheckScope())
			{
				var newValue = EditorGUILayout.Toggle(ENABLE_VERBOSE_LOGGING_LABEL, EnableVerboseLogging);

				if (scope.changed)
				{
					EnableVerboseLogging = newValue;
				}
			}
		}

		private static void DrawProjectPrefsGUI(string value = "")
		{
			EditorGUILayout.LabelField(PROJECT_REFERENCES_HEADER, EditorStyles.boldLabel);

			// Genesis Command-Line Folder
			EditorGUILayout.HelpBox(GENESIS_CLI_DIRECTORY_DESCRIPTION, MessageType.Info);

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField(GENESIS_CLI_INSTALL_FOLDER_LABEL, GUILayout.MaxWidth(110f));
				using (var scope = new EditorGUI.ChangeCheckScope())
				{
					var newValue = EditorGUILayout.TextField(GenesisCLIInstallationFolder);
					if (scope.changed)
					{
						GenesisCLIInstallationFolder = newValue;
					}
				}

				var currentFolder = GenesisCLIInstallationFolder;
				if (GUILayoutTools.DrawFolderPickerLayout(ref currentFolder, GENESIS_CLI_FOLDER_SELECT_TITLE))
				{
					GenesisCLIInstallationFolder = currentFolder;
				}
			}
		}

		#endregion

		#region Command-line

		/// <summary>
		/// Returns the absolute path to the <see cref="EditorConstants.GENESIS_EXECUTABLE"/> executable.
		/// </summary>
		public static string GetExecutablePath()
		{
			return Path.Combine(Path.GetFullPath(GenesisCLIInstallationFolder), EditorConstants.GENESIS_EXECUTABLE);
		}

		/// <summary>
		/// Returns the absolute path to the working folder the the <see cref="EditorConstants.GENESIS_EXECUTABLE"/>.
		/// </summary>
		public static string GetWorkingPath()
		{
			return Path.GetFullPath(GenesisCLIInstallationFolder);
		}

		#endregion
	}
}
