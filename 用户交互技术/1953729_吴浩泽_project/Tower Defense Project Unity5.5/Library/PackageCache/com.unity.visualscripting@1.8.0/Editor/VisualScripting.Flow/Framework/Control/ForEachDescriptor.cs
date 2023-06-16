namespace Unity.VisualScripting
{
    [Descriptor(typeof(ForEach))]
    public class ForEachDescriptor : UnitDescriptor<ForEach>
    {
        public ForEachDescriptor(ForEach unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            if (unit.dictionary && port == unit.currentItem)
            {
                description.label = "Value";
                description.summary = "The value of the current item of the loop.";
            }
        }
    }
}
