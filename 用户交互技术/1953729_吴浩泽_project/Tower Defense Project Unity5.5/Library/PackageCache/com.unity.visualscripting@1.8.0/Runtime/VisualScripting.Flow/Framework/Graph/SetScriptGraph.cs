using JetBrains.Annotations;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Set a ScriptGraph to a ScriptMachine
    /// </summary>
    [TypeIcon(typeof(FlowGraph))]
    public sealed class SetScriptGraph : SetGraph<FlowGraph, ScriptGraphAsset, ScriptMachine>
    {
        /// <summary>
        /// The type of object that handles the graph.
        /// </summary>
        [Serialize, Inspectable, UnitHeaderInspectable, UsedImplicitly]
        public ScriptGraphContainerType containerType { get; set; }

        protected override bool isGameObject => containerType == ScriptGraphContainerType.GameObject;
    }
}
