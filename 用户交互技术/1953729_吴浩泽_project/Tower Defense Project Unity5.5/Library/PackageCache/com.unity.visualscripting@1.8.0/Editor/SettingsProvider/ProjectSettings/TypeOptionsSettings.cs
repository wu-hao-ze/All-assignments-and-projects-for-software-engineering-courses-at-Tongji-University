using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class TypeOptionsSettings
    {
        private readonly PluginConfigurationItemMetadata _typeOptionsMetadata;

        private bool _showTypeOption = false;
        private const string TitleTypeOption = "Type Options";
        private const string DescriptionTypeOption = "Choose the types you want to use for variables and nodes.\n"
            + "MonoBehaviour types are always included.";

        private static class Styles
        {
            public static readonly GUIStyle background;
            public static readonly GUIStyle defaultsButton;
            public const float OptionsWidth = 250;

            static Styles()
            {
                background = new GUIStyle(LudiqStyles.windowBackground);
                background.padding = new RectOffset(20, 20, 20, 20);

                defaultsButton = new GUIStyle("Button");
                defaultsButton.padding = new RectOffset(10, 10, 4, 4);
            }
        }

        public TypeOptionsSettings(BoltCoreConfiguration coreConfig)
        {
            _typeOptionsMetadata = coreConfig.GetMetadata(nameof(coreConfig.typeOptions));
        }

        public void OnGUI()
        {
            _showTypeOption = EditorGUILayout.Foldout(_showTypeOption, new GUIContent(TitleTypeOption, DescriptionTypeOption));

            if (_showTypeOption)
            {
                GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));

                float height =
                    LudiqGUI.GetInspectorHeight(null, _typeOptionsMetadata, Styles.OptionsWidth, GUIContent.none);

                EditorGUI.BeginChangeCheck();

                var position = GUILayoutUtility.GetRect(Styles.OptionsWidth, height);

                LudiqGUI.Inspector(_typeOptionsMetadata, position, GUIContent.none);

                if (EditorGUI.EndChangeCheck())
                {
                    _typeOptionsMetadata.SaveImmediately(true);
                    Codebase.UpdateSettings();
                }

                if (GUILayout.Button("Reset to Defaults", Styles.defaultsButton) && EditorUtility.DisplayDialog("Reset Included Types", "Reset the included types to their defaults?", "Reset to Default", "Cancel"))
                {
                    _typeOptionsMetadata.Reset(true);
                    _typeOptionsMetadata.SaveImmediately(true);
                }

                LudiqGUI.EndVertical();
            }
        }
    }
}
