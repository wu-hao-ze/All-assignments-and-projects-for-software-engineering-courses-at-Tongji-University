namespace Unity.VisualScripting
{
    [Descriptor(typeof(ScriptMachine))]
    public sealed class FlowMachineDescriptor : MachineDescriptor<ScriptMachine, MachineDescription>
    {
        public FlowMachineDescriptor(ScriptMachine target) : base(target) { }
    }
}
