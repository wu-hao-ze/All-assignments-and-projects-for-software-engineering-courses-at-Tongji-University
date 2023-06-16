using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class UnknownInspector : Inspector
    {
        public UnknownInspector(Metadata metadata) : base(metadata) { }

        private string GetMessage(GUIContent label)
        {
            var labelText = ProcessLabel(metadata, label).text;

            if (!string.IsNullOrEmpty(labelText))
            {
                return $"{labelText}: No inspector for '{metadata.definedType.DisplayName()}'.";
            }
            else
            {
                return $"No inspector for '{metadata.definedType.DisplayName()}'.";
            }
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position.height -= EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.HelpBox(position, GetMessage(label), MessageType.Warning);
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = LudiqGUIUtility.GetHelpBoxHeight(GetMessage(label), MessageType.Warning, width);
            height += EditorGUIUtility.standardVerticalSpacing;
            return height;
        }
    }
}
