namespace Unity.VisualScripting
{
    [Descriptor(typeof(SetMember))]
    public class SetMemberDescriptor : MemberUnitDescriptor<SetMember>
    {
        public SetMemberDescriptor(SetMember unit) : base(unit) { }

        protected override ActionDirection direction => ActionDirection.Set;

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            if (port == unit.assign)
            {
                description.label = "Set";
                description.summary = "The entry point to set the value.";
            }
            else if (port == unit.assigned)
            {
                description.label = "On Set";
                description.summary = "The action to call once the value has been set.";
            }
            else if (port == unit.output)
            {
                description.summary = unit.member.info.Summary();
            }
        }
    }
}
