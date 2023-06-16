using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(char))]
    public class CharInspector : Inspector
    {
        public CharInspector(Metadata metadata) : base(metadata) { }

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var newString = EditorGUI.TextField(position, ((char)metadata.value).ToString());

            char newValue;

            if (string.IsNullOrEmpty(newString))
            {
                newValue = (char)0;
            }
            else
            {
                newValue = newString[0];
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }
    }
}
