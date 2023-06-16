using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(IGraph))]
    public class GraphEditor : Inspector
    {
        public GraphEditor(Metadata metadata) : base(metadata) { }

        private Metadata titleMetadata => metadata[nameof(IGraph.title)];

        private Metadata summaryMetadata => metadata[nameof(IGraph.summary)];

        protected virtual EditorTexture icon => metadata.definedType.Icon();

        protected IGraph graph => (IGraph)metadata.value;

        protected override float GetHeight(float width, GUIContent label)
        {
            return GetHeaderHeight(width);
        }

        protected float GetHeaderHeight(float width)
        {
            return LudiqGUI.GetHeaderHeight
                (
                    this,
                    titleMetadata,
                    summaryMetadata,
                    icon,
                    width
                );
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            OnHeaderGUI(position);
        }

        protected void OnHeaderGUI(Rect position)
        {
            LudiqGUI.OnHeaderGUI
                (
                    titleMetadata,
                    summaryMetadata,
                    icon,
                    position,
                    ref y
                );
        }
    }
}
