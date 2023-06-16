using UnityEngine;
using UnityEditor;

namespace Unity.VisualScripting
{
    internal class EditorPreferencesProviderView : SettingsProvider
    {
        private const string Path = "Preferences/Visual Scripting";
        private const string Title = "Visual Scripting";
        private const string ID = "Bolt";
        private readonly GUIStyle marginStyle = new GUIStyle() { margin = new RectOffset(10, 10, 10, 10) };

        public EditorPreferencesProviderView() : base(Path, SettingsScope.User)
        {
            label = Title;
        }

        private void EnsureConfig()
        {
            if (BoltCore.instance == null || BoltCore.Configuration == null)
            {
                PluginContainer.Initialize();
            }
        }

        public override void OnGUI(string searchContext)
        {
            EnsureConfig();

            GUILayout.BeginVertical(marginStyle);

            // happens when opening unity with the settings window already opened. there's a delay until the singleton is assigned
            if (BoltCore.instance == null)
            {
                EditorGUILayout.HelpBox("Loading Configuration...", MessageType.Info);
                return;
            }

            var instance = (BoltProduct)ProductContainer.GetProduct(ID);

            instance.configurationPanel.PreferenceItem();

            GUILayout.EndVertical();
        }
    }
}
