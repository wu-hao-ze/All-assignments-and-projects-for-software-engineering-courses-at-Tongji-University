using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class VectorInspector : Inspector
    {
        protected VectorInspector(Metadata metadata) : base(metadata) { }

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
        }

        public static class Styles
        {
            public static float compactThreshold = 120;
            public static float compactSpacing = 2;
        }
    }
}
