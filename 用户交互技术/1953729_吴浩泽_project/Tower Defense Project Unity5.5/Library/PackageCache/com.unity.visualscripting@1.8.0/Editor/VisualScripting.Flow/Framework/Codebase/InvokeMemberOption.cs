namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(InvokeMember))]
    public class InvokeMemberOption : MemberUnitOption<InvokeMember>
    {
        public InvokeMemberOption() : base() { }

        public InvokeMemberOption(InvokeMember unit) : base(unit) { }

        protected override ActionDirection direction => ActionDirection.Any;

        public override string SearchResultLabel(string query)
        {
            return base.SearchResultLabel(query) + $" ({unit.member.methodBase.DisplayParameterString(unit.member.targetType)})";
        }

        protected override string Label(bool human)
        {
            return base.Label(human) + $" ({unit.member.methodBase.SelectedParameterString(unit.member.targetType, human)})";
        }

        protected override string Haystack(bool human)
        {
            if (!human && member.isConstructor)
            {
                return base.Label(human);
            }
            else
            {
                return $"{targetType.SelectedName(human)}{(human ? ": " : ".")}{base.Label(human)}";
            }
        }
    }
}
