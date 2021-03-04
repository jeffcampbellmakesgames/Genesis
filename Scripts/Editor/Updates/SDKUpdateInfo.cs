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

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Represents an immutable state of update information between a local and remote semantic version of an SDK.
	/// </summary>
	internal sealed class SDKUpdateInfo
	{
		public readonly SDKUpdateState updateState;
		public readonly string localVersionString;
		public readonly string remoteVersionString;

		public SDKUpdateInfo(string localVersionString, string remoteVersionString)
		{
			this.localVersionString = localVersionString.Trim();
			this.remoteVersionString = remoteVersionString.Trim();

			if (remoteVersionString != string.Empty)
			{
				var localVersion = new Version(localVersionString);

				// Parse remote version to remove any tag prefixes and 'v' character
				var vIndex = remoteVersionString.IndexOf('v');
				var normalizedRemoteVersion = remoteVersionString.Substring(vIndex + 1);
				var remoteVersion = new Version(normalizedRemoteVersion);

				switch (remoteVersion.CompareTo(localVersion))
				{
					case 1:
						updateState = SDKUpdateState.UpdateAvailable;
						break;
					case 0:
						updateState = SDKUpdateState.UpToDate;
						break;
					case -1:
						updateState = SDKUpdateState.AheadOfLatestRelease;
						break;
				}
			}
			else
			{
				updateState = SDKUpdateState.NoConnection;
			}
		}
	}
}
