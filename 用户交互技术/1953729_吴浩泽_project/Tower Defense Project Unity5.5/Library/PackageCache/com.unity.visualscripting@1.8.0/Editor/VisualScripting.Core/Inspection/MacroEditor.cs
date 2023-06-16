using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(IMacro))]
    public class MacroEditor : Inspector
    {
        public MacroEditor(Metadata metadata) : base(metadata) { }

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;
            height += EditorGUIUtility.standardVerticalSpacing;
            height += GetButtonHeight(width);
            height += EditorGUIUtility.standardVerticalSpacing;
            return height;
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, GUIContent.none);

            y += EditorGUIUtility.standardVerticalSpacing;

            var buttonPosition = new Rect
                (
                position.x,
                y,
                position.width,
                GetButtonHeight(position.width)
                );

            OnButtonGUI(buttonPosition);

            y += buttonPosition.height;

            EndBlock(metadata);
        }

        private float GetButtonHeight(float width)
        {
            return EditorGUIUtility.singleLineHeight + 3;
        }

        private void OnButtonGUI(Rect sourcePosition)
        {
            if (GUI.Button(sourcePosition, "Edit Graph"))
            {
                GraphWindow.OpenActive(GraphReference.New((IMacro)metadata.value, true));
            }
        }
    }
}
