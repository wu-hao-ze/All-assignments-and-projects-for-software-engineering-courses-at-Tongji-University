using JetBrains.Annotations;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Check if a GameObject or ScriptMachine has a ScriptGraph
    /// </summary>
    [TypeIcon(typeof(FlowGraph))]
    [UnitCategory("Graphs/Graph Nodes")]
    public sealed class HasScriptGraph : HasGraph<FlowGraph, ScriptGraphAsset, ScriptMachine>
    {
        /// <summary>
        /// The type of object that handles the graph.
        /// </summary>
        [Serialize, Inspectable, UnitHeaderInspectable, UsedImplicitly]
        public ScriptGraphContainerType containerType { get; set; }

        protected override bool isGameObject => containerType == ScriptGraphContainerType.GameObject;
    }
}
