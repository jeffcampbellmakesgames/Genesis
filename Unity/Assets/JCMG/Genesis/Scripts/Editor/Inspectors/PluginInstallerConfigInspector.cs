using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace JCMG.Genesis.Editor.Inspectors
{
	/// <summary>
	/// A custom inspector for <see cref="PluginInstallerConfig"/> that enables easy selection and configuration of
	/// Third-Party Genesis Plugins.
	/// </summary>
	[CustomEditor(typeof(PluginInstallerConfig))]
	public sealed class PluginInstallerConfigInspector : UnityEditor.Editor
	{
		private ReorderableList _pluginPathsReorderableList;

		// UI
		private const string PLUGIN_PATHS_TITLE = "Plugin Zip Paths";
		private const string PLUGIN_PATHS_DESCRIPTION =
			"Contains a collection of Unity GUIDs to ZIP files containing Genesis Plugins. These are rendered below as " +
			"relative file paths.";

		private const string PLUGIN_ZIP_FILE_PICKER_TITLE = "Select Plugin Zip Path";
		private const string ZIP_FILE_EXT = "zip";

		// Properties
		private const string DISPLAY_NAME_PROP = "_displayName";
		private const string PLUGIN_ZIP_GUIDS_PROP = "_pluginZipGUIDs";

		private void OnEnable()
		{
			_pluginPathsReorderableList =
				new ReorderableList(serializedObject,
					serializedObject.FindProperty(PLUGIN_ZIP_GUIDS_PROP),
					true,
					true,
					true,
					true);
			_pluginPathsReorderableList.drawHeaderCallback += DrawPluginPathListHeader;
			_pluginPathsReorderableList.drawElementCallback += DrawPluginPathListElement;
		}

		private void OnDisable()
		{
			_pluginPathsReorderableList.drawHeaderCallback -= DrawPluginPathListHeader;
			_pluginPathsReorderableList.drawElementCallback -= DrawPluginPathListElement;
		}

		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			var pluginInstallerConfig = (PluginInstallerConfig) target;
			EditorGUILayout.PropertyField(serializedObject.FindProperty(DISPLAY_NAME_PROP));

			EditorGUILayout.HelpBox(PLUGIN_PATHS_DESCRIPTION, MessageType.Info);
			_pluginPathsReorderableList.DoLayoutList();

			using (new EditorGUI.DisabledScope(!pluginInstallerConfig.CanAttemptPluginInstall))
			{
				if (GUILayout.Button(EditorConstants.INSTALL_PLUGIN_BUTTON_TEXT))
				{
					pluginInstallerConfig.InstallPlugins();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawPluginPathListHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, PLUGIN_PATHS_TITLE, EditorStyles.boldLabel);
		}

		private void DrawPluginPathListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			const float FILE_PICKER_BUTTON_WIDTH = 40f;

			var element = _pluginPathsReorderableList.serializedProperty.GetArrayElementAtIndex(index);
			var elementGuid = element.stringValue;

			var textFieldRect = new Rect(rect)
			{
				width = rect.width - FILE_PICKER_BUTTON_WIDTH,
				height = rect.height
			};
			var folderPickerRect = new Rect
			{
				x = textFieldRect.width + FILE_PICKER_BUTTON_WIDTH,
				y = rect.y,
				width = FILE_PICKER_BUTTON_WIDTH,
				height = rect.height
			};

			var assetPath = AssetDatabase.GUIDToAssetPath(elementGuid);
			EditorGUI.TextField(textFieldRect, assetPath);

			GUILayoutTools.DrawGUIDFilePicker(
				folderPickerRect,
				ref elementGuid,
				PLUGIN_ZIP_FILE_PICKER_TITLE,
				ZIP_FILE_EXT);

			element.stringValue = elementGuid;
		}
	}
}
