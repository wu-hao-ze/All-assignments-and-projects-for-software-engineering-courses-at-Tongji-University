using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(Ray))]
    public class RayInspector : Inspector
    {
        public RayInspector(Metadata metadata) : base(metadata) { }

        protected override float GetHeight(float width, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var originPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            var directionPosition = new Rect
                (
                position.x,
                originPosition.yMax + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            using (LudiqGUIUtility.labelWidth.Override(16))
            {
                originPosition = PrefixLabel(metadata["origin"], originPosition, new GUIContent("O", "Origin"));
                directionPosition = PrefixLabel(metadata["direction"], directionPosition, new GUIContent("D", "Direction"));
            }

            Vector3 newOrigin;
            Vector3 newDirection;

            if (wideMode)
            {
                newOrigin = EditorGUI.Vector3Field(originPosition, GUIContent.none, (Vector3)metadata["origin"].value);
                newDirection = EditorGUI.Vector3Field(directionPosition, GUIContent.none, (Vector3)metadata["direction"].value);
            }
            else
            {
                newOrigin = LudiqGUI.CompactVector3Field(originPosition, GUIContent.none, (Vector3)metadata["origin"].value);
                newDirection = LudiqGUI.CompactVector3Field(directionPosition, GUIContent.none, (Vector3)metadata["direction"].value);
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = new Ray(newOrigin, newDirection);
            }
        }

        public override float GetAdaptiveWidth()
        {
            return 125;
        }
    }
}
