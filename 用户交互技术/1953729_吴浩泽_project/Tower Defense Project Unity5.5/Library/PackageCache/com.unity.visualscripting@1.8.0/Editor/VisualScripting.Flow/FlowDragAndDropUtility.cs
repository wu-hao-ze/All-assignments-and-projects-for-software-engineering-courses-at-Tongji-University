using UnityEditor;

namespace Unity.VisualScripting
{
    public static class FlowDragAndDropUtility
    {
        public static bool AcceptsScript(IGraph graph)
        {
            // Can't drag a graph into itself
            return DragAndDrop.objectReferences[0] is ScriptGraphAsset graphAsset && graph != graphAsset.graph;
        }
    }
}
