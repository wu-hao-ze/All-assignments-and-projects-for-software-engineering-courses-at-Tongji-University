namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(SetMember))]
    public class SetMemberOption : MemberUnitOption<SetMember>
    {
        public SetMemberOption() : base() { }

        public SetMemberOption(SetMember unit) : base(unit) { }

        protected override ActionDirection direction => ActionDirection.Set;

        protected override bool ShowValueOutputsInFooter()
        {
            return false;
        }
    }
}
