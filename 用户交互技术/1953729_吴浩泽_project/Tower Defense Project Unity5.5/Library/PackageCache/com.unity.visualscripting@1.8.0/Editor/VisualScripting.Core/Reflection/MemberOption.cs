namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(Member))]
    public sealed class MemberOption : DocumentedOption<Member>
    {
        public MemberOption(Member member) : this(member, ActionDirection.Any, false) { }

        public MemberOption(Member member, ActionDirection direction, bool expectingBoolean)
        {
            Ensure.That(nameof(member)).IsNotNull(member);

            value = member;

            documentation = member.info.Documentation();

            UnityAPI.Async(() => icon = member.pseudoDeclaringType.Icon());

            if (member.isPseudoInherited)
            {
                style = FuzzyWindow.Styles.optionWithIconDim;
            }

            if (member.isInvocable)
            {
                label = $"{member.info.DisplayName(direction, expectingBoolean)} ({member.methodBase.DisplayParameterString(member.targetType)})";
            }
            else
            {
                label = member.info.DisplayName(direction, expectingBoolean);
            }
        }

        private static string MemberNameWithTargetType(Member member, ActionDirection direction, bool expectingBoolean)
        {
            return $"{member.targetType.DisplayName()}{(BoltCore.Configuration.humanNaming ? ": " : ".")}{member.info.DisplayName(direction, expectingBoolean)}";
        }

        public static string Haystack(Member member, ActionDirection direction, bool expectingBoolean)
        {
            return MemberNameWithTargetType(member, direction, expectingBoolean);
        }

        public static string SearchResultLabel(Member member, string query, ActionDirection direction, bool expectingBoolean)
        {
            var label = SearchUtility.HighlightQuery(Haystack(member, direction, expectingBoolean), query);

            if (member.isInvocable)
            {
                label += $" ({member.methodBase.DisplayParameterString(member.targetType)})";
            }

            return label;
        }
    }
}
