using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(Ray2D))]
    public class Ray2DInspector : Inspector
    {
        public Ray2DInspector(Metadata metadata) : base(metadata) { }

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

            var newOrigin = EditorGUI.Vector2Field(originPosition, GUIContent.none, (Vector2)metadata["origin"].value);
            var newDirection = EditorGUI.Vector2Field(directionPosition, GUIContent.none, (Vector2)metadata["direction"].value);

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = new Ray2D(newOrigin, newDirection);
            }
        }

        public override float GetAdaptiveWidth()
        {
            return 100;
        }
    }
}
