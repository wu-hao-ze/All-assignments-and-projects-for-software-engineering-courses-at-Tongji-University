using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(GraphOutput))]
    public class GraphOutputInspector : Inspector
    {
        public GraphOutputInspector(Metadata metadata) : base(metadata) { }

        private Metadata graphMetadata => metadata[nameof(GraphOutput.graph)];
        private Metadata controlOutputDefinitionsMetadata => graphMetadata[nameof(FlowGraph.controlOutputDefinitions)];
        private Metadata valueOutputDefinitionsMetadata => graphMetadata[nameof(FlowGraph.valueOutputDefinitions)];

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            if (graphMetadata.value != null)
            {
                height += GetControlOutputDefinitionsHeight(width);

                height += EditorGUIUtility.standardVerticalSpacing;

                height += GetValueOutputDefinitionsHeight(width);
            }

            return height;
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            BeginLabeledBlock(metadata, position, label);

            if (graphMetadata.value != null)
            {
                EditorGUI.BeginChangeCheck();

                LudiqGUI.Inspector(controlOutputDefinitionsMetadata, position.VerticalSection(ref y, GetControlOutputDefinitionsHeight(position.width)));

                y += EditorGUIUtility.standardVerticalSpacing;

                LudiqGUI.Inspector(valueOutputDefinitionsMetadata, position.VerticalSection(ref y, GetValueOutputDefinitionsHeight(position.width)));

                if (EditorGUI.EndChangeCheck())
                {
                    ((FlowGraph)graphMetadata.value).PortDefinitionsChanged();
                }
            }

            EndBlock(metadata);
        }

        private float GetControlOutputDefinitionsHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, controlOutputDefinitionsMetadata, width);
        }

        private float GetValueOutputDefinitionsHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, valueOutputDefinitionsMetadata, width);
        }
    }
}
