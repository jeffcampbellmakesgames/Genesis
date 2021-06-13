using System;
using System.Diagnostics;
using System.IO;
using Unity.SharpZipLib.Utils;
using UnityEditor;
using UnityEditor.Callbacks;
using Debug = UnityEngine.Debug;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// An editor helper to assist with updating the Genesis.CLI application in-place.
	/// </summary>
	internal class AutoUpdateDetector
	{
		// Logs
		public const string GENESIS_CLI_OUT_OF_DATE = EditorConstants.LOG_PREFIX +
			"Genesis CLI application detected at v{0}), current package version is v{1}, update required!";

		public const string UPDATING_GENESIS =
			EditorConstants.LOG_PREFIX + "Updating Genesis CLI application...!";

		// Modal Dialog
		public const string GENESIS_CLI_TITLE_NOT_FOUND = "Genesis CLI Not Found";
		public const string GENESIS_CLI_MSG_NOT_FOUND = "A Genesis CLI installation path was found, but did not have an " +
		                                                "installation present. " +
		                                                GENESIS_CLI_INSTALLATION_INSTRUCTIONS;
		public const string GENESIS_CLI_TITLE_UPDATE_REQUIRED = "Genesis CLI Update Required";

		public const string GENESIS_CLI_MSG_UPDATE_REQUIRED =
			"The installed version of Genesis.CLI is out of date and requires an update. " +
			GENESIS_CLI_INSTALLATION_INSTRUCTIONS;
		public const string GENESIS_CLI_INSTALLATION_INSTRUCTIONS =
			" Press OK to update or Cancel to continue. ";

		[DidReloadScripts]
		private static void OnScriptsReloaded()
		{
			TryUpdateGenesisCLI();
		}

		/// <summary>
		/// Returns true if the Genesis CLI application is installed and up-to-date, otherwise false.
		/// </summary>
		public static bool TryUpdateGenesisCLI(bool autoUpdateWithoutPrompt = false)
		{
			var isInstalledAndUpToDate = true;
			var appZipAssetPath = EditorConstants.AppZipAssetPath;
			var projectPath = FileTools.GetProjectPath();
			var fullAppZipPath = Path.GetFullPath(Path.Combine(projectPath, appZipAssetPath));

			if (!File.Exists(fullAppZipPath))
			{
				Debug.LogWarning(EditorConstants.GENESIS_CLI_ZIP_NOT_FOUND);

				isInstalledAndUpToDate = false;
			}
			else
			{
				// If there is already a Genesis CLI installation present, check to see if the version is older than
				// the current Genesis package version, if so an update is required.
				var doUpdateFromZip = false;
				var executablePath = GenesisPreferences.GetExecutablePath();
				var title = string.Empty;
				var message = string.Empty;
				if (File.Exists(executablePath))
				{
					var existingApplicationVersionRaw = FileVersionInfo.GetVersionInfo(executablePath).FileVersion;
					var existingApplicationVersion = new Version(existingApplicationVersionRaw);
					var genesisVersion = new Version(VersionConstants.VERSION);
					if (genesisVersion > existingApplicationVersion)
					{
						Debug.LogFormat(
							GENESIS_CLI_OUT_OF_DATE,
							existingApplicationVersion,
							genesisVersion);

						isInstalledAndUpToDate = false;
						doUpdateFromZip = true;
						title = GENESIS_CLI_TITLE_UPDATE_REQUIRED;
						message = GENESIS_CLI_MSG_UPDATE_REQUIRED;
					}
				}
				// Otherwise if the user has specified an installation folder, but the application is not present
				// initiate an update
				else if (!string.IsNullOrEmpty(GenesisPreferences.GenesisCLIInstallationFolder) &&
				         Directory.Exists(GenesisPreferences.GetWorkingPath()))
				{
					isInstalledAndUpToDate = false;
					doUpdateFromZip = true;
					title = GENESIS_CLI_TITLE_NOT_FOUND;
					message = GENESIS_CLI_MSG_NOT_FOUND;
				}

				// If an update is required and the user agrees, extract the latest update
				if (doUpdateFromZip)
				{
					var didConfirm = false;
					if (autoUpdateWithoutPrompt)
					{
						didConfirm = true;
					}
					else
					{
						didConfirm = EditorUtility.DisplayDialog(
							title,
							message,
							EditorConstants.DIALOG_OK,
							EditorConstants.DIALOG_CANCEL);
					}

					if (didConfirm)
					{
						Debug.Log(UPDATING_GENESIS);

						var fullInstallationPath = Path.GetFullPath(GenesisPreferences.GenesisCLIInstallationFolder);

						FileTools.ExtractZipContents(fullAppZipPath, fullInstallationPath);

						isInstalledAndUpToDate = true;
					}
				}
			}

			return isInstalledAndUpToDate;
		}
	}
}
