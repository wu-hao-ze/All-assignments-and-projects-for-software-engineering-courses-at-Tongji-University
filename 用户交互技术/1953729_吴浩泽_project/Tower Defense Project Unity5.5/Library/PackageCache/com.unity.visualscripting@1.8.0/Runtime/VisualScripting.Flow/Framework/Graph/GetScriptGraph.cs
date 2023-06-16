namespace Unity.VisualScripting
{
    /// <summary>
    /// Get a ScriptGraphAsset from a GameObject
    /// </summary>
    [TypeIcon(typeof(FlowGraph))]
    public class GetScriptGraph : GetGraph<FlowGraph, ScriptGraphAsset, ScriptMachine> { }
}
