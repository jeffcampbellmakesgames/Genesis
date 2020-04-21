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

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Helper methods for checking remote SDK updates
	/// </summary>
	internal static class SDKUpdateTools
	{
		/// <summary>
		/// Json response fields from remote latest release
		/// </summary>
		private struct ResponseData
		{
			/// <summary>
			/// The SemVer tag of the latest release.
			/// </summary>
			#pragma warning disable 649
			// ReSharper disable once InconsistentNaming
			public string tag_name;
			#pragma warning restore 649
		}

		// Title
		private const string TITLE_FORMAT = "{0} Update";

		// Body Content
		private const string UPDATE_AVAILABLE_BODY_FORMAT = "A newer version of {0} is available!\n\n" +
		                                     "Currently installed version: {1}\n" +
		                                     "New version: {2}";

		private const string UP_TO_DATE_BODY_FORMAT = "{0} is up to date (v{1}).";

		private const string HAS_NEWER_VERSION_THAN_RELEASED_BODY_FORMAT =
			"Your {0} version seems to be newer than the latest release?!?\n\n" +
			"Currently installed version: {1}\n" +
			"Latest release: {2}";

		private const string CANNOT_CONTACT_GITHUB_BODY_FORMAT = "Could not request latest {0} version!\n\n" +
		                                                         "Make sure that you are connected to the internet.\n";

		// Button Prompts
		private const string OK_BUTTON = "OK";
		private const string CANCEL_BUTTON = "Cancel";
		private const string TRY_AGAIN_BUTTON = "Try Again";
		private const string SHOW_IN_GITHUB_BUTTON = "Show in GitHub";

		public static void CheckUpdateInfo(string sdkName, string githubLatestReleaseUrl)
		{
			var info = GetUpdateInfo(githubLatestReleaseUrl);
			DisplayUpdateInfo(info, sdkName, githubLatestReleaseUrl);
		}

		public static SDKUpdateInfo GetUpdateInfo(string githubLatestReleaseUrl)
		{
			var localVersion = GetLocalVersion();
			var remoteVersion = GetRemoteVersion(githubLatestReleaseUrl);
			return new SDKUpdateInfo(localVersion, remoteVersion);
		}

		private static string GetLocalVersion()
		{
			return VersionConstants.VERSION;
		}

		private static string GetRemoteVersion(string githubLatestReleaseUrl)
		{
			try
			{
				return JsonUtility.FromJson<ResponseData>(RequestLatestRelease(githubLatestReleaseUrl)).tag_name;
			}
			catch
			{
				// ignored
			}

			return string.Empty;
		}

		private static string RequestLatestRelease(string githubLatestReleaseUrl)
		{
			var response = string.Empty;
			using (var www = UnityWebRequest.Get(githubLatestReleaseUrl))
			{
				var asyncOperation = www.SendWebRequest();
				while (!asyncOperation.isDone)
				{
				}

				if (!www.isNetworkError && !www.isHttpError)
				{
					response = asyncOperation.webRequest.downloadHandler.text;
				}
			}

			return response;
		}

		private static void DisplayUpdateInfo(SDKUpdateInfo info, string sdkName, string githubLatestReleaseUrl)
		{
			switch (info.updateState)
			{
				case SDKUpdateState.UpdateAvailable:
					if (EditorUtility.DisplayDialog(
						GetDialogTitle(sdkName),
						GetUpdateAvailableBody(sdkName, info),
						SHOW_IN_GITHUB_BUTTON,
						CANCEL_BUTTON))
					{
						Application.OpenURL(githubLatestReleaseUrl);
					}

					break;
				case SDKUpdateState.UpToDate:
					EditorUtility.DisplayDialog(
						GetDialogTitle(sdkName),
						GetUpToDateBody(sdkName, info),
						OK_BUTTON);
					break;
				case SDKUpdateState.AheadOfLatestRelease:
					if (EditorUtility.DisplayDialog(
						GetDialogTitle(sdkName),
						GetNewerVersionWarningBody(sdkName, info),
						SHOW_IN_GITHUB_BUTTON,
						CANCEL_BUTTON))
					{
						Application.OpenURL(githubLatestReleaseUrl);
					}

					break;
				case SDKUpdateState.NoConnection:
					if (EditorUtility.DisplayDialog(
						GetDialogTitle(sdkName),
						GetRemoteWarningBody(sdkName),
						TRY_AGAIN_BUTTON,
						CANCEL_BUTTON))
					{
						CheckUpdateInfo(sdkName, githubLatestReleaseUrl);
					}

					break;
			}
		}

		private static string GetDialogTitle(string sdkName)
		{
			return string.Format(TITLE_FORMAT, sdkName);
		}

		private static string GetUpdateAvailableBody(string sdkName, SDKUpdateInfo updateInfo)
		{
			return string.Format(
				UPDATE_AVAILABLE_BODY_FORMAT,
				sdkName,
				updateInfo.localVersionString,
				updateInfo.remoteVersionString);
		}

		private static string GetUpToDateBody(string sdkName, SDKUpdateInfo updateInfo)
		{
			return string.Format(UP_TO_DATE_BODY_FORMAT, sdkName, updateInfo.localVersionString);
		}

		private static string GetNewerVersionWarningBody(string sdkName, SDKUpdateInfo updateInfo)
		{
			return string.Format(
				HAS_NEWER_VERSION_THAN_RELEASED_BODY_FORMAT,
				sdkName,
				updateInfo.localVersionString,
				updateInfo.remoteVersionString);
		}

		private static string GetRemoteWarningBody(string sdkName)
		{
			return string.Format(CANNOT_CONTACT_GITHUB_BODY_FORMAT, sdkName);
		}
	}
}
