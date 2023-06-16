namespace Unity.VisualScripting
{
    [Descriptor(typeof(GetMember))]
    public class GetMemberDescriptor : MemberUnitDescriptor<GetMember>
    {
        public GetMemberDescriptor(GetMember unit) : base(unit) { }

        protected override ActionDirection direction => ActionDirection.Get;

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            if (port == unit.value)
            {
                description.summary = unit.member.info.Summary();
            }
        }
    }
}
