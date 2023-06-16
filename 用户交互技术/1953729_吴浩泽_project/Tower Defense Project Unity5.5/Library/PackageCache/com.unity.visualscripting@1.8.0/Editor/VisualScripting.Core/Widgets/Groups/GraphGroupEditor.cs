using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(GraphGroup))]
    public sealed class GraphGroupEditor : Inspector
    {
        public GraphGroupEditor(Metadata metadata) : base(metadata) { }

        private Metadata labelMetadata => metadata[nameof(GraphGroup.label)];

        private Metadata commentMetadata => metadata[nameof(GraphGroup.comment)];

        private EditorTexture icon => metadata.definedType.Icon();

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            height += GetHeaderHeight(width);
            height += Styles.inspectorPadding.top;
            height += GetInspectorHeight(width);
            height += Styles.inspectorPadding.bottom;

            return height;
        }

        private float GetHeaderHeight(float width)
        {
            return LudiqGUI.GetHeaderHeight
                (
                    this,
                    labelMetadata,
                    commentMetadata,
                    icon,
                    width,
                    true
                );
        }

        private float GetInspectorHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, metadata, width, GUIContent.none);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            OnHeaderGUI(position);

            y += Styles.inspectorPadding.top;

            var inspectorPosition = new Rect
                (
                position.x + Styles.inspectorPadding.left,
                y,
                position.width - Styles.inspectorPadding.left - Styles.inspectorPadding.right,
                position.height
                );

            OnInspectorGUI(inspectorPosition);
        }

        private void OnHeaderGUI(Rect position)
        {
            LudiqGUI.OnHeaderGUI
                (
                    labelMetadata,
                    commentMetadata,
                    icon,
                    position,
                    ref y,
                    true
                );
        }

        private void OnInspectorGUI(Rect inspectorPosition)
        {
            LudiqGUI.Inspector(metadata, inspectorPosition, GUIContent.none);
        }

        public static class Styles
        {
            static Styles()
            {
                inspectorPadding = new RectOffset(10, 10, 0, 10);
            }

            public static readonly RectOffset inspectorPadding;
        }
    }
}
