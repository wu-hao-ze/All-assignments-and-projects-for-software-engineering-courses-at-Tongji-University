using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(INesterStateTransition))]
    public class NesterStateTransitionEditor : StateTransitionEditor
    {
        public NesterStateTransitionEditor(Metadata metadata) : base(metadata) { }

        private Metadata nestMetadata => metadata[nameof(INesterStateTransition.nest)];

        private Metadata graphMetadata => nestMetadata[nameof(IGraphNest.graph)];

        protected override GraphReference headerReference => reference.ChildReference((INesterStateTransition)metadata.value, false);

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
