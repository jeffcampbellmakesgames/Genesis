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
using UnityEditor;

namespace JCMG.Genesis.Editor
{
	internal sealed class AssembliesConfigDrawer : AbstractSettingsDrawer
	{
		/// <summary>
		/// The display title for this drawer
		/// </summary>
		public override string Title => TITLE;

		/// <summary>
		/// The ascending order in which this drawer should be shown with regards to other <see cref="ISettingsDrawer"/>
		/// instances
		/// </summary>
		public override int Order => 1;

		private readonly AssembliesConfig _config;

		// UI
		private const string TITLE = "Assemblies";
		private const string DO_USE_WHITE_LIST_LABEL = "Do Use Whitelist";
		private const string DO_USE_WHITE_LIST_DESCRIPTION = "If enabled, searching via reflection for Data Providers " +
		                                                     "will be limited to the array of assemblies below. Otherwise " +
		                                                     "all loaded assemblies will be searched.";

		private const string ASSEMBLY_WHITE_LIST_LABEL = "Assembly Whitelist";
		private const string ASSEMBLY_WHITE_LIST_DESCRIPTION = "The comma delimited array of assemblies that searching " +
		                                                       "via reflection for Data Providers should be limited to.";

		public AssembliesConfigDrawer()
		{
			_config = new AssembliesConfig();
		}

		/// <summary>Initializes any setup for the drawer prior to rendering any GUI.</summary>
		/// <param name="settings"></param>
		public override void Initialize(GenesisSettings settings)
		{
			base.Initialize(settings);

			_config.Configure(settings);
		}

		protected override void DrawContentBody(GenesisSettings settings)
		{
			// Do Use white-list
			EditorGUILayout.HelpBox(DO_USE_WHITE_LIST_DESCRIPTION, MessageType.Info);
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField(DO_USE_WHITE_LIST_LABEL);

				using (var scope = new EditorGUI.ChangeCheckScope())
				{
					var newValue = EditorGUILayout.Toggle(_config.DoUseWhitelistOfAssemblies);

					if (scope.changed)
					{
						_config.DoUseWhitelistOfAssemblies = newValue;

						EditorUtility.SetDirty(settings);
					}
				}
			}

			// White-Listed Assemblies
			EditorGUILayout.HelpBox(ASSEMBLY_WHITE_LIST_DESCRIPTION, MessageType.Info);
			using (new EditorGUI.DisabledScope(!_config.DoUseWhitelistOfAssemblies))
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.LabelField(ASSEMBLY_WHITE_LIST_LABEL);

					using (var scope = new EditorGUI.ChangeCheckScope())
					{
						var newValue = EditorGUILayout.TextField(_config.RawWhiteListedAssemblies);

						if (scope.changed)
						{
							_config.RawWhiteListedAssemblies = newValue;

							EditorUtility.SetDirty(settings);
						}
					}
				}
			}
		}
	}
}
