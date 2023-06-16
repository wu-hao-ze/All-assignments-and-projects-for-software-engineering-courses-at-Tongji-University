namespace Unity.VisualScripting
{
    [Descriptor(typeof(StateMachine))]
    public sealed class StateMachineDescriptor : MachineDescriptor<StateMachine, MachineDescription>
    {
        public StateMachineDescriptor(StateMachine target) : base(target) { }
    }
}
