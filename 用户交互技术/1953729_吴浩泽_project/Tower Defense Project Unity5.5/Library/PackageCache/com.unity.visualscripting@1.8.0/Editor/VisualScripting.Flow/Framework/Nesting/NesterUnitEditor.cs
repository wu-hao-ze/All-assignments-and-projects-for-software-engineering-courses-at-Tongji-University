using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(INesterUnit))]
    public class NesterUnitEditor : UnitEditor
    {
        public NesterUnitEditor(Metadata metadata) : base(metadata) { }

        private Metadata nestMetadata => metadata[nameof(INesterUnit.nest)];

        private Metadata graphMetadata => nestMetadata[nameof(IGraphNest.graph)];

        protected override GraphReference headerReference => reference.ChildReference((INesterUnit)metadata.value, false);

        protected override Metadata headerTitleMetadata => graphMetadata[nameof(IGraph.title)];

        protected override Metadata headerSummaryMetadata => graphMetadata[nameof(IGraph.summary)];

        protected override float GetInspectorHeight(float width)
        {
            return LudiqGUI.GetEditorHeight(this, nestMetadata, width);
        }

        protected override void OnInspectorGUI(Rect position)
        {
            LudiqGUI.Editor(nestMetadata, position);
        }
    }
}
