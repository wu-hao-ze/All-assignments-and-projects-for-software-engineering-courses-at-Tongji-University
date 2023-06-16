namespace Unity.VisualScripting
{
    [Descriptor(typeof(TriggerCustomEvent))]
    public class TriggerCustomEventDescriptor : UnitDescriptor<TriggerCustomEvent>
    {
        public TriggerCustomEventDescriptor(TriggerCustomEvent trigger) : base(trigger) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            var index = unit.arguments.IndexOf(port as ValueInput);

            if (index >= 0)
            {
                description.label = "Arg. " + index;
            }
        }
    }
}
