using System.Linq;
using UnityEditor;

namespace JCMG.Genesis.Editor.Inspectors
{
	[CustomEditor(typeof(GenesisSettings))]
	internal sealed class GenesisSettingsInspector : UnityEditor.Editor
	{
		private static readonly ISettingsDrawer[] PREFERENCES_DRAWERS;

		static GenesisSettingsInspector()
		{
			PREFERENCES_DRAWERS = ReflectionTools.GetAllImplementingInstancesOfInterface<ISettingsDrawer>().ToArray();
		}
		/// <summary>
		///   <para>Implement this function to make a custom inspector.</para>
		/// </summary>
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
		}
	}
}
