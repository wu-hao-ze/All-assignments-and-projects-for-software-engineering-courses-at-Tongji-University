namespace Unity.VisualScripting
{
    [Descriptor(typeof(CustomEvent))]
    public class CustomEventDescriptor : EventUnitDescriptor<CustomEvent>
    {
        public CustomEventDescriptor(CustomEvent @event) : base(@event) { }

        protected override string DefinedSubtitle()
        {
            return null;
        }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            var index = unit.argumentPorts.IndexOf(port as ValueOutput);

            if (index >= 0)
            {
                description.label = "Arg. " + index;
            }
        }
    }
}
