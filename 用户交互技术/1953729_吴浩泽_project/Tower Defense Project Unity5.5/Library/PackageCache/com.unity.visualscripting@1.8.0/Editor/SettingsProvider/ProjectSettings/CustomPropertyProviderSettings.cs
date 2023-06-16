using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class CustomPropertyProviderSettings
    {
        private const string Title = "Custom Inspector Properties";
        private const string ButtonLabel = "Generate";

        public void OnGUI()
        {
            GUILayout.Space(5f);

            GUILayout.Label(Title, EditorStyles.boldLabel);

            GUILayout.Space(5f);

            string label = "Inspectors in Visual Scripting plugins can handle many custom types besides Unity primites and objects. ";
            label += "However, to be compatible with your custom editor drawers, some additional property provider scripts must be generated. ";

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(EditorGUIUtility.IconContent("console.infoicon"), GUILayout.ExpandWidth(true));
            GUILayout.Box(label, EditorStyles.wordWrappedLabel);
            GUILayout.EndHorizontal();

            if (GUILayout.Button(ButtonLabel, Styles.defaultsButton))
            {
                SerializedPropertyProviderProvider.instance.GenerateProviderScripts();
                EditorUtility.DisplayDialog("Custom Inspector Generation", "Custom inspector generation has completed successfully.", "OK");
            }
        }

        private static class Styles
        {
            static Styles()
            {
                defaultsButton = new GUIStyle("Button");
                defaultsButton.padding = new RectOffset(10, 10, 4, 4);
            }

            public static readonly GUIStyle defaultsButton;
        }
    }
}
