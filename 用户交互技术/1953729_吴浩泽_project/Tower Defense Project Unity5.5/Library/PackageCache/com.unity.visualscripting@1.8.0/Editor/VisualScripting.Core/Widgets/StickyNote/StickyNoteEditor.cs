using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(StickyNote))]
    public sealed class StickyNoteEditor : Inspector
    {
        public StickyNoteEditor(Metadata metadata) : base(metadata) { }

        private Metadata titleMetadata => metadata[nameof(StickyNote.title)];

        private Metadata bodyMetadata => metadata[nameof(StickyNote.body)];

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
                    titleMetadata,
                    bodyMetadata,
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
                    titleMetadata,
                    bodyMetadata,
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
