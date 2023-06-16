namespace Unity.VisualScripting
{
    [Descriptor(typeof(WaitForFlow))]
    public class WaitForFlowDescriptor : UnitDescriptor<WaitForFlow>
    {
        public WaitForFlowDescriptor(WaitForFlow unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            if (port is ControlInput && unit.awaitedInputs.Contains((ControlInput)port))
            {
                description.showLabel = false;
            }
        }
    }
}
