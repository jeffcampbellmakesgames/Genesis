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

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// A base class for an EditorGUI Drawer for a set of properties on a <see cref="GenesisSettings"/> instance.
	/// Enables custom drawing of zero or more settings that will appear on the `Project Settings\Genesis` UI.
	/// </summary>
	public abstract class AbstractSettingsDrawer : ISettingsDrawer
	{
		protected bool _drawContent = true;

		public abstract string Title { get; }

		protected abstract void DrawContentBody(GenesisSettings settings);

		public virtual void Initialize(GenesisSettings settings)
		{

		}

		public virtual void DrawHeader(GenesisSettings settings)
		{
			// No-op
		}

		public virtual void DrawContent(GenesisSettings settings)
		{
			// The default implementation for draw content enables its content to be collapsed via a foldout header bar.
			_drawContent = EditorGUILayoutTools.DrawSectionHeaderToggle(Title, _drawContent);
			if (!_drawContent)
			{
				return;
			}

			EditorGUILayoutTools.BeginSectionContent();
			DrawContentBody(settings);
			EditorGUILayoutTools.EndSectionContent();
		}
	}
}
