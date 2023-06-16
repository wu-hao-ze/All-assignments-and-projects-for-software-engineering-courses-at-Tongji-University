namespace Unity.VisualScripting
{
    [Descriptor(typeof(ScriptGraphAsset))]
    public sealed class FlowMacroDescriptor : MacroDescriptor<ScriptGraphAsset, MacroDescription>
    {
        public FlowMacroDescriptor(ScriptGraphAsset target) : base(target) { }
    }
}
