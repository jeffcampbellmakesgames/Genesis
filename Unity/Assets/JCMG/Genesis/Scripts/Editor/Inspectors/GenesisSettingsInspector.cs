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
