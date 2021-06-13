using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// A scriptable object that contains plugin installation configuration for third-party Genesis plugins.
	/// </summary>
	[CreateAssetMenu(
		fileName = "",
		menuName = EditorConstants.CREATE_ASSET_MENU_ITEM_PREFIX + "PluginInstallerConfig")]
	public class PluginInstallerConfig : ScriptableObject
	{
		/// <summary>
		/// The display name for this plugin installer.
		/// </summary>
		public string DisplayName => string.IsNullOrEmpty(_displayName) ? name : _displayName;

		/// <summary>
		/// Returns true if there are any non-null or empty plugin GUIDs.
		/// </summary>
		public bool HasPluginZipGUIDS => _pluginZipGUIDs != null &&
		                                 _pluginZipGUIDs.Any(x => !string.IsNullOrEmpty(x));

		/// <summary>
		/// Returns true if a plugin install can be attempted, otherwise false.
		/// </summary>
		public virtual bool CanAttemptPluginInstall =>
			HasPluginZipGUIDS && Directory.Exists(GenesisPreferences.GetWorkingPath());

		[Tooltip("The display name for this plugin installer.")]
		[SerializeField]
		private string _displayName;

		/// <summary>
		/// A collection of Unity file GUIDs to zips of .Net Core Genesis Plugins.
		/// </summary>
		[Tooltip("A collection of file paths to zips of .Net Core Genesis Plugins.")]
		[SerializeField]
		private List<string> _pluginZipGUIDs;

		private const string SKIPPING_MISSING_OR_NONEXISTENT_FILE_WARNING =
			EditorConstants.LOG_PREFIX +
			"Skipping installing file [{0} with guid [{1}] as it is missing or nonexistent.";

		private const string INVALID_PLUGIN_INSTALLER_FILE_WARNING =
			EditorConstants.LOG_PREFIX + "Cannot install file [{0}] to Plugins install directory as it is not a Zip " +
			"file. Please reference only Zip files from a PluginInstallerConfig";

		private const string PLUGIN_INSTALL_SUCCEEDED = EditorConstants.LOG_PREFIX + "Successfully installed plugins for [{0}].";
		private const string PLUGIN_INSTALL_FAILED = EditorConstants.LOG_PREFIX + "Failed to install plugins for [{0}].\n\n{1}";

		/// <summary>
		/// Attempts to install plugins from this <see cref="PluginInstallerConfig"/>.
		/// </summary>
		public virtual void InstallPlugins()
		{
			try
			{
				// Ensure there is a valid target directory to install to and that there are any referenced plugin ZIPs
				var workingDirectory = GenesisPreferences.GetWorkingPath();
				if (!HasPluginZipGUIDS || !Directory.Exists(workingDirectory))
				{
					return;
				}

				// For each plugin zip GUID, attempt to extract them to the default plugin directory in the
				// user-specified installation path.
				var pluginDestinationDirectory = Path.Combine(workingDirectory, EditorConstants.DEFAULT_PLUGIN_PATH);
				foreach (var pluginPath in _pluginZipGUIDs)
				{
					var assetPath = AssetDatabase.GUIDToAssetPath(pluginPath);
					if (string.IsNullOrEmpty(assetPath))
					{
						Debug.LogWarningFormat(
							SKIPPING_MISSING_OR_NONEXISTENT_FILE_WARNING,
							assetPath,
							pluginPath);
						continue;
					}

					// If the referenced file is a ZIP file, extract it to the default plugin directory
					// Otherwise issue a warning.
					var fullAssetPath = Path.GetFullPath(Path.Combine(FileTools.GetProjectPath(), assetPath));
					if (fullAssetPath.EndsWith(EditorConstants.ZIP_EXTENSION))
					{
						FileTools.ExtractZipContents(fullAssetPath, pluginDestinationDirectory);
					}
					else
					{
						Debug.LogWarningFormat(INVALID_PLUGIN_INSTALLER_FILE_WARNING, fullAssetPath);
					}
				}

				Debug.LogFormat(PLUGIN_INSTALL_SUCCEEDED, DisplayName);
			}
			catch (Exception ex)
			{
				Debug.LogFormat(PLUGIN_INSTALL_FAILED, DisplayName, ex);
			}
		}
	}
}
