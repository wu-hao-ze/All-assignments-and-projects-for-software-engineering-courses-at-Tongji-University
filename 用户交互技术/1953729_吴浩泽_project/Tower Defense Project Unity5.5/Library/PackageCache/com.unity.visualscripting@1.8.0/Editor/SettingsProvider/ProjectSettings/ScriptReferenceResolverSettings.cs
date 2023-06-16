using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class ScriptReferenceResolverSettings
    {
        private const string Title = "Script Reference Resolver";
        private const string ButtonLabel = "Fix Missing Scripts";

        public void OnGUI()
        {
            GUILayout.Space(5f);

            GUILayout.Label(Title, EditorStyles.boldLabel);

            GUILayout.Space(5f);

            if (GUILayout.Button(ButtonLabel, Styles.defaultsButton))
            {
                ScriptReferenceResolver.Run(ScriptReferenceResolver.Mode.Dialog);
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
