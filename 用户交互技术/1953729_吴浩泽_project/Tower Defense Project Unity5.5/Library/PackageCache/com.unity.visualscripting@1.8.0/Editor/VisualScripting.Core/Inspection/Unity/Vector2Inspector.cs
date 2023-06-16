using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(Vector2))]
    public class Vector2Inspector : VectorInspector
    {
        public Vector2Inspector(Metadata metadata) : base(metadata) { }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            Vector2 newValue;

            if (adaptiveWidth)
            {
                newValue = LudiqGUI.AdaptiveVector2Field(position, GUIContent.none, (Vector2)metadata.value);
            }
            else if (position.width <= Styles.compactThreshold)
            {
                newValue = LudiqGUI.CompactVector2Field(position, GUIContent.none, (Vector2)metadata.value);
            }
            else
            {
                newValue = EditorGUI.Vector2Field(position, GUIContent.none, (Vector2)metadata.value);
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        protected override void OnEditorPrefGUI(Rect position, GUIContent label)
        {
            BeginBlock(metadata, position);

            Vector2 newValue = EditorGUI.Vector2Field(position, label, (Vector2)metadata.value);

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        public override float GetAdaptiveWidth()
        {
            var vector = (Vector2)metadata.value;

            return LudiqGUI.GetTextFieldAdaptiveWidth(vector.x) + LudiqStyles.compactHorizontalSpacing +
                LudiqGUI.GetTextFieldAdaptiveWidth(vector.y) + LudiqStyles.compactHorizontalSpacing;
        }
    }
}
