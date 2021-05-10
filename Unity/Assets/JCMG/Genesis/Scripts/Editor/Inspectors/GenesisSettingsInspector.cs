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
using Genesis.Shared;
using UnityEditor;
using UnityEngine;

namespace JCMG.Genesis.Editor.Inspectors
{
	[CustomEditor(typeof(GenesisSettings))]
	internal sealed class GenesisSettingsInspector : UnityEditor.Editor
	{
		private static readonly ISettingsDrawer[] PREFERENCES_DRAWERS;

		private const string ACTIONS_TITLE = "Actions";
		private const string GENERATE_BUTTON_TEXT = "Generate";

		static GenesisSettingsInspector()
		{
			PREFERENCES_DRAWERS = ReflectionTools.GetAllImplementingInstancesOfInterface<ISettingsDrawer>()
				.OrderBy(x => x.Order)
				.ToArray();
		}

		public override void OnInspectorGUI()
		{
			var settings = (GenesisSettings)target;
			using (var scope = new EditorGUI.ChangeCheckScope())
			{
				for (var i = 0; i < PREFERENCES_DRAWERS.Length; i++)
				{
					var preferencesDrawer = PREFERENCES_DRAWERS[i];
					preferencesDrawer.Initialize(settings);
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

			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField(ACTIONS_TITLE, EditorStyles.boldLabel);
			if(GUILayout.Button(GENERATE_BUTTON_TEXT))
			{
				GenesisCLIRunner.RunCodeGeneration(new []{ settings });
			}
		}
	}
}
