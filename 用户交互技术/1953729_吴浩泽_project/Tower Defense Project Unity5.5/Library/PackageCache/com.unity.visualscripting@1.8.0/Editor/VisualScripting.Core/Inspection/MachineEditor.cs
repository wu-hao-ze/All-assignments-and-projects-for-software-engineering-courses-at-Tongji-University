using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(IMachine))]
    public class MachineEditor : Inspector
    {
        public MachineEditor(Metadata metadata) : base(metadata) { }

        private Metadata nestMetadata => metadata[nameof(IMachine.nest)];

        private Metadata graphMetadata => nestMetadata[nameof(IGraphNest.graph)];

        protected Metadata headerTitleMetadata => graphMetadata[nameof(IGraph.title)];

        protected Metadata headerSummaryMetadata => graphMetadata[nameof(IGraph.summary)];

        protected virtual bool showHeader => graphMetadata.value != null;

        protected virtual bool showConfiguration => false;

        protected sealed override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            if (showHeader)
            {
                height += GetHeaderHeight(width);
            }

            height += GetNestHeight(width);

            if (showConfiguration)
            {
                height += GetConfigurationHeight(width);
            }

            return height;
        }

        protected sealed override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, GUIContent.none);

            if (showHeader)
            {
                var headerPosition = position;
                headerPosition.x = 0;
                headerPosition.width = LudiqGUIUtility.currentInspectorWidthWithoutScrollbar;
                OnHeaderGUI(headerPosition);
            }

            var nestPosition = position.VerticalSection(ref y, LudiqGUI.GetEditorHeight(this, nestMetadata, position.width));
            OnNestGUI(nestPosition);

            if (showConfiguration)
            {
                OnConfigurationGUI(position);
            }

            EndBlock(metadata);
        }

        protected virtual float GetHeaderHeight(float width)
        {
            return LudiqGUI.GetHeaderHeight(this, headerTitleMetadata, headerSummaryMetadata, null, LudiqGUIUtility.currentInspectorWidthWithoutScrollbar);
        }

        protected virtual void OnHeaderGUI(Rect headerPosition)
        {
            LudiqGUI.OnHeaderGUI(headerTitleMetadata, headerSummaryMetadata, null, headerPosition, ref y);
        }

        protected virtual float GetNestHeight(float width)
        {
            return LudiqGUI.GetEditorHeight(this, nestMetadata, width);
        }

        protected virtual void OnNestGUI(Rect nestPosition)
        {
            LudiqGUI.Editor(nestMetadata, nestPosition);
        }

        protected virtual float GetConfigurationHeight(float width)
        {
            return 0;
        }

        protected virtual void OnConfigurationGUI(Rect position)
        {
        }
    }
}
