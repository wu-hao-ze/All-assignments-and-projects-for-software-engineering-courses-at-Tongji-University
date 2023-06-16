using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(Vector3))]
    public class Vector3Inspector : VectorInspector
    {
        public Vector3Inspector(Metadata metadata) : base(metadata) { }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            Vector3 newValue;

            if (adaptiveWidth)
            {
                newValue = LudiqGUI.AdaptiveVector3Field(position, GUIContent.none, (Vector3)metadata.value);
            }
            else if (position.width <= Styles.compactThreshold)
            {
                newValue = LudiqGUI.CompactVector3Field(position, GUIContent.none, (Vector3)metadata.value);
            }
            else
            {
                newValue = EditorGUI.Vector3Field(position, GUIContent.none, (Vector3)metadata.value);
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        public override float GetAdaptiveWidth()
        {
            var vector = (Vector3)metadata.value;

            return LudiqGUI.GetTextFieldAdaptiveWidth(vector.x) + LudiqStyles.compactHorizontalSpacing +
                LudiqGUI.GetTextFieldAdaptiveWidth(vector.y) + LudiqStyles.compactHorizontalSpacing +
                LudiqGUI.GetTextFieldAdaptiveWidth(vector.z) + LudiqStyles.compactHorizontalSpacing;
        }
    }
}
