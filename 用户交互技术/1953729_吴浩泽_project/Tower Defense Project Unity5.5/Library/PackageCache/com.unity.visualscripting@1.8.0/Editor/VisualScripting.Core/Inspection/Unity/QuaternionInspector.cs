using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(Quaternion))]
    public class QuaternionInspector : Inspector
    {
        public QuaternionInspector(Metadata metadata) : base(metadata) { }

        protected override float GetHeight(float width, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var value = (Quaternion)metadata.value;

            var vector = new Vector4(value.x, value.y, value.z, value.w);

            Vector4 newVector;

            if (adaptiveWidth)
            {
                newVector = LudiqGUI.AdaptiveVector4Field(position, GUIContent.none, vector);
            }
            else if (position.width <= VectorInspector.Styles.compactThreshold)
            {
                newVector = LudiqGUI.CompactVector4Field(position, GUIContent.none, vector);
            }
            else
            {
                newVector = EditorGUI.Vector4Field(position, GUIContent.none, vector);
            }

            if (EndBlock(metadata))
            {
                var newValue = new Quaternion(newVector.x, newVector.y, newVector.z, newVector.w);

                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }
    }
}
