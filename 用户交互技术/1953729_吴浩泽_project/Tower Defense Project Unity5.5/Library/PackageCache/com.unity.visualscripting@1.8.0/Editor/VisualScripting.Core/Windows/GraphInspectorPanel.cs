using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class GraphInspectorPanel : ISidebarPanelContent
    {
        public IGraphContext context { get; }

        public object sidebarControlHint => typeof(GraphInspectorPanel);

        public GUIContent titleContent { get; }

        public Vector2 minSize => new Vector2(275, 200);

        public GraphInspectorPanel(IGraphContext context)
        {
            this.context = context;

            titleContent = new GUIContent("Graph Inspector", BoltCore.Icons.inspectorWindow?[IconSize.Small]);
        }

        public void OnGUI(Rect position)
        {
            var y = position.y;

            EditorGUIUtility.hierarchyMode = true; // For the label width to be correct, like in the inspector

            if (context != null)
            {
                context.BeginEdit();

                var selectionSize = context.selection.Count;

                if (selectionSize == 0)
                {
                    var graphPanelPosition = position.VerticalSection(ref y, GetGraphPanelHeight(position.width));

                    LudiqGUI.Editor(context.graphMetadata, graphPanelPosition);
                }
                else if (selectionSize == 1)
                {
                    var selectionPanelPosition = position.VerticalSection(ref y, GetSelectionPanelHeight(position.width));

                    LudiqGUI.Editor(context.selectionMetadata, selectionPanelPosition);
                }
                else if (selectionSize > 1)
                {
                    var noMultiEditPosition = new Rect
                        (
                        position.x,
                        y,
                        position.width,
                        GetNoMultiEditHeight(position.width)
                        );

                    EditorGUI.HelpBox(noMultiEditPosition, NoMultiEditMessage, MessageType.Info);
                }

                context.EndEdit();
            }
            else
            {
                var noGraphSelectedPosition = new Rect
                    (
                    position.x,
                    y,
                    position.width,
                    GetNoGraphSelectedHeight(position.width)
                    );

                EditorGUI.HelpBox(noGraphSelectedPosition, NoGraphSelectedMessage, MessageType.Info);
            }
        }

        public float GetHeight(float width)
        {
            EditorGUIUtility.hierarchyMode = true; // For the label width to be correct, like in the inspector

            var height = 0f;

            if (context != null)
            {
                context.BeginEdit();

                var selectionSize = context.selection.Count;

                if (selectionSize == 0)
                {
                    height += GetGraphPanelHeight(width);
                }
                else if (selectionSize == 1)
                {
                    height += GetSelectionPanelHeight(width);
                }
                else if (selectionSize > 1)
                {
                    height += GetNoMultiEditHeight(width);
                }

                context.EndEdit();
            }
            else
            {
                height += GetNoGraphSelectedHeight(width);
            }

            return height;
        }

        private float GetGraphPanelHeight(float width)
        {
            return LudiqGUI.GetEditorHeight(null, context.graphMetadata, width);
        }

        private float GetSelectionPanelHeight(float width)
        {
            return LudiqGUI.GetEditorHeight(null, context.selectionMetadata, width);
        }

        private float GetNoMultiEditHeight(float width)
        {
            return LudiqGUIUtility.GetHelpBoxHeight(NoMultiEditMessage, MessageType.Info, width);
        }

        private float GetNoGraphSelectedHeight(float width)
        {
            return LudiqGUIUtility.GetHelpBoxHeight(NoGraphSelectedMessage, MessageType.Info, width);
        }

        private const string NoGraphSelectedMessage = "No graph selected.";

        private const string NoMultiEditMessage = "Multi-element editing is not supported.";
    }
}
