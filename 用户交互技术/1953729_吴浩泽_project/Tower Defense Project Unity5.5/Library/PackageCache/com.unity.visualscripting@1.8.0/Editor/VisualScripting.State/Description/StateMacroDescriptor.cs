namespace Unity.VisualScripting
{
    [Descriptor(typeof(StateGraphAsset))]
    public sealed class StateMacroDescriptor : MacroDescriptor<StateGraphAsset, MacroDescription>
    {
        public StateMacroDescriptor(StateGraphAsset target) : base(target) { }
    }
}
