namespace Unity.VisualScripting
{
    [Descriptor(typeof(Expose))]
    public class ExposeDescriptor : UnitDescriptor<Expose>
    {
        public ExposeDescriptor(Expose unit) : base(unit) { }

        protected override string DefinedTitle()
        {
            return $"Expose {unit.type.DisplayName()}";
        }

        protected override string DefinedSurtitle()
        {
            return "Expose";
        }

        protected override string DefinedShortTitle()
        {
            return unit.type.DisplayName();
        }

        protected override EditorTexture DefinedIcon()
        {
            return unit.type.Icon();
        }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            if (port is ValueOutput && unit.members.TryGetValue((ValueOutput)port, out Member member))
            {
                description.label = member.info.HumanName();
                description.summary = member.info.Summary();
            }
        }
    }
}
