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

using Genesis.Core;

namespace JCMG.Genesis.Editor
{
	internal sealed class NewLineSettingsDrawer : AbstractSettingsDrawer
	{
		/// <summary>
		/// The display title for this drawer.
		/// </summary>
		public override string Title => TITLE;

		/// <summary>
		/// The ascending order in which this drawer should be shown with regards to other <see cref="ISettingsDrawer"/>
		/// instances
		/// </summary>
		public override int Order => 3;

		private LineEndingConfig _lineEndingConfig;

		private const string TITLE = "Convert Line Endings";

		public NewLineSettingsDrawer()
		{
			_lineEndingConfig = new LineEndingConfig();
		}

		/// <summary>
		/// Initializes any setup for the drawer prior to rendering any GUI.
		/// </summary>
		/// <param name="settings"></param>
		public override void Initialize(GenesisSettings settings)
		{
			_lineEndingConfig.Configure(settings);
		}

		protected override void DrawContentBody(GenesisSettings settings)
		{
			_lineEndingConfig.LineEnding = (LineEndingMode)UnityEditor.EditorGUILayout.EnumPopup(
				typeof(LineEndingMode).Name,
				_lineEndingConfig.LineEnding);
		}
	}
}
