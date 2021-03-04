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

using Genesis.Shared;

namespace JCMG.Genesis.Editor.Plugins
{
	internal sealed class TargetDirectorySettingsDrawer : AbstractSettingsDrawer
	{
		/// <summary>
		/// The display title for this drawer
		/// </summary>
		public override string Title => TITLE;

		/// <summary>
		/// The ascending order in which this drawer should be shown with regards to other <see cref="ISettingsDrawer"/>
		/// instances
		/// </summary>
		public override int Order => 4;

		private readonly TargetDirectoryConfig _directoryConfig;

		private const string TITLE = "Output Directory";

		public TargetDirectorySettingsDrawer()
		{
			_directoryConfig = new TargetDirectoryConfig();
		}

		/// <summary>
		/// Initializes any setup for the drawer prior to rendering any GUI.
		/// </summary>
		/// <param name="settings"></param>
		public override void Initialize(GenesisSettings settings)
		{
			_directoryConfig.Configure(settings);
		}

		protected override void DrawContentBody(GenesisSettings settings)
		{
			_directoryConfig.TargetDirectory = UnityEditor.EditorGUILayout.TextField(_directoryConfig.TargetDirectory);
		}
	}
}
