namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(GetMember))]
    public class GetMemberOption : MemberUnitOption<GetMember>
    {
        public GetMemberOption() : base() { }

        public GetMemberOption(GetMember unit) : base(unit) { }

        protected override ActionDirection direction => ActionDirection.Get;
    }
}
