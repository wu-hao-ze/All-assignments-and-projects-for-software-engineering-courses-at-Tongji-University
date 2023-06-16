using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(Vector4))]
    public class Vector4Inspector : VectorInspector
    {
        public Vector4Inspector(Metadata metadata) : base(metadata) { }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            Vector4 newValue;

            if (adaptiveWidth)
            {
                newValue = LudiqGUI.AdaptiveVector4Field(position, GUIContent.none, (Vector4)metadata.value);
            }
            else if (position.width <= Styles.compactThreshold)
            {
                newValue = LudiqGUI.CompactVector4Field(position, GUIContent.none, (Vector4)metadata.value);
            }
            else
            {
                newValue = EditorGUI.Vector4Field(position, GUIContent.none, (Vector4)metadata.value);
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        public override float GetAdaptiveWidth()
        {
            var vector = (Vector4)metadata.value;

            return LudiqGUI.GetTextFieldAdaptiveWidth(vector.x) + LudiqStyles.compactHorizontalSpacing +
                LudiqGUI.GetTextFieldAdaptiveWidth(vector.y) + LudiqStyles.compactHorizontalSpacing +
                LudiqGUI.GetTextFieldAdaptiveWidth(vector.z) + LudiqStyles.compactHorizontalSpacing +
                LudiqGUI.GetTextFieldAdaptiveWidth(vector.w) + LudiqStyles.compactHorizontalSpacing;
        }
    }
}
