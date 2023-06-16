namespace Unity.VisualScripting
{
    /// <summary>
    /// Get a list of all the ScriptGraphs from a GameObject
    /// </summary>
    [TypeIcon(typeof(FlowGraph))]
    public class GetScriptGraphs : GetGraphs<FlowGraph, ScriptGraphAsset, ScriptMachine> { }
}
