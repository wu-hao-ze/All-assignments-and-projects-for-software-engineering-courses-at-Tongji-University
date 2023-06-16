using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class UnknownEditor : Inspector
    {
        public UnknownEditor(Metadata metadata) : base(metadata) { }

        private string message => $"No inspector for '{metadata.definedType.DisplayName()}'.";

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position.height -= EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.HelpBox(position, message, MessageType.Warning);
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = LudiqGUIUtility.GetHelpBoxHeight(message, MessageType.Warning, width);
            height += EditorGUIUtility.standardVerticalSpacing;
            return height;
        }
    }
}
