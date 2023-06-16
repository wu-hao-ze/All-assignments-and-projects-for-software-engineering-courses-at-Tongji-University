using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(Guid))]
    public sealed class GuidInspector : Inspector
    {
        public GuidInspector(Metadata metadata) : base(metadata) { }

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            Guid newGuid;

            var fieldPosition = new Rect
                (
                position.x,
                position.y,
                position.width - Styles.buttonSpacing - Styles.buttonWidth,
                EditorGUIUtility.singleLineHeight
                );

            var buttonPosition = new Rect
                (
                position.xMax - Styles.buttonWidth,
                position.y,
                Styles.buttonWidth,
                EditorGUIUtility.singleLineHeight
                );

            try
            {
                newGuid = new Guid(EditorGUI.DelayedTextField(fieldPosition, ((Guid)metadata.value).ToString()));
            }
            catch
            {
                newGuid = (Guid)metadata.value;
            }

            if (GUI.Button(buttonPosition, "New GUID"))
            {
                newGuid = Guid.NewGuid();
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newGuid;
            }
        }

        public static class Styles
        {
            public static readonly float buttonWidth = 70;
            public static readonly float buttonSpacing = 3;
        }
    }
}
