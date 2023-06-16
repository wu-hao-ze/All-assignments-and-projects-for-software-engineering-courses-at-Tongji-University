using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(GraphInput))]
    public class GraphInputInspector : Inspector
    {
        public GraphInputInspector(Metadata metadata) : base(metadata) { }

        private Metadata graphMetadata => metadata[nameof(GraphInput.graph)];
        private Metadata controlInputDefinitionsMetadata => graphMetadata[nameof(FlowGraph.controlInputDefinitions)];
        private Metadata valueInputDefinitionsMetadata => graphMetadata[nameof(FlowGraph.valueInputDefinitions)];

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            if (graphMetadata.value != null)
            {
                height += GetControlInputDefinitionsHeight(width);

                height += EditorGUIUtility.standardVerticalSpacing;

                height += GetValueInputDefinitionsHeight(width);
            }

            return height;
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            BeginLabeledBlock(metadata, position, label);

            if (graphMetadata.value != null)
            {
                EditorGUI.BeginChangeCheck();

                LudiqGUI.Inspector(controlInputDefinitionsMetadata, position.VerticalSection(ref y, GetControlInputDefinitionsHeight(position.width)));

                y += EditorGUIUtility.standardVerticalSpacing;

                LudiqGUI.Inspector(valueInputDefinitionsMetadata, position.VerticalSection(ref y, GetValueInputDefinitionsHeight(position.width)));

                if (EditorGUI.EndChangeCheck())
                {
                    ((FlowGraph)graphMetadata.value).PortDefinitionsChanged();
                }
            }

            EndBlock(metadata);
        }

        private float GetControlInputDefinitionsHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, controlInputDefinitionsMetadata, width);
        }

        private float GetValueInputDefinitionsHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, valueInputDefinitionsMetadata, width);
        }
    }
}
