using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(KeyValuePair<,>))]
    public sealed class KeyValuePairInspector : Inspector
    {
        public KeyValuePairInspector(Metadata metadata) : base(metadata) { }

        protected override float GetHeight(float width, GUIContent label)
        {
            return Mathf.Max(GetKeyHeight(width), GetValueHeight(width)) + Styles.topPadding;
        }

        private float GetKeyHeight(float width)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private float GetValueHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, metadata["Value"], width, GUIContent.none);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var keyPosition = new Rect
                (
                position.x,
                position.y + Styles.topPadding,
                (position.width - Styles.spacing) / 2,
                GetKeyHeight(position.width)
                );

            var valuePosition = new Rect
                (
                keyPosition.xMax + Styles.spacing,
                position.y + Styles.topPadding,
                (position.width - Styles.spacing) / 2,
                GetValueHeight(position.width)
                );

            EditorGUI.BeginDisabledGroup(true);
            OnKeyGUI(keyPosition);
            OnValueGUI(valuePosition);
            EditorGUI.EndDisabledGroup();

            EndBlock(metadata);
        }

        public void OnKeyGUI(Rect keyPosition)
        {
            LudiqGUI.Inspector(metadata["Key"], keyPosition, GUIContent.none);
        }

        public void OnValueGUI(Rect valuePosition)
        {
            LudiqGUI.Inspector(metadata["Value"], valuePosition, GUIContent.none);
        }

        public static class Styles
        {
            public static readonly float topPadding = 2;
            public static readonly float spacing = 5;
        }
    }
}
