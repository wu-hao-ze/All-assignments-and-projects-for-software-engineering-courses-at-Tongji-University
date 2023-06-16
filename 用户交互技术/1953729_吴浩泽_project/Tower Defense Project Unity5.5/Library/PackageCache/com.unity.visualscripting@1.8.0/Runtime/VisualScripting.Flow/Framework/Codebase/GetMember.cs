namespace Unity.VisualScripting
{
    /// <summary>
    /// Gets the value of a field or property via reflection.
    /// </summary>
    public sealed class GetMember : MemberUnit
    {
        public GetMember() { }

        public GetMember(Member member) : base(member) { }

        [DoNotSerialize]
        [MemberFilter(Fields = true, Properties = true, WriteOnly = false)]
        public Member getter
        {
            get
            {
                return member;
            }
            set
            {
                member = value;
            }
        }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            value = ValueOutput(member.type, nameof(value), Value);

            if (member.isPredictable)
            {
                value.Predictable();
            }

            if (member.requiresTarget)
            {
                Requirement(target, value);
            }
        }

        protected override bool IsMemberValid(Member member)
        {
            return member.isAccessor && member.isGettable;
        }

        private object Value(Flow flow)
        {
            var target = member.requiresTarget ? flow.GetValue(this.target, member.targetType) : null;

            return member.Get(target);
        }

        #region Analytics

        public override AnalyticsIdentifier GetAnalyticsIdentifier()
        {
            var aid = new AnalyticsIdentifier
            {
                Identifier = $"{member.targetType.FullName}.{member.name}(Get)",
                Namespace = member.targetType.Namespace
            };
            aid.Hashcode = aid.Identifier.GetHashCode();
            return aid;
        }

        #endregion
    }
}
