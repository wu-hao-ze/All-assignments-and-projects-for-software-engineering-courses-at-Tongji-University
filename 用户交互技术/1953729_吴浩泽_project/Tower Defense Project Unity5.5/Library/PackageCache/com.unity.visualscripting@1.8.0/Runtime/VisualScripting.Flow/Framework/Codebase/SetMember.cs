namespace Unity.VisualScripting
{
    /// <summary>
    /// Sets the value of a field or property via reflection.
    /// </summary>
    public sealed class SetMember : MemberUnit
    {
        public SetMember() : base() { }

        public SetMember(Member member) : base(member) { }

        /// <summary>
        /// Whether the target should be output to allow for chaining.
        /// </summary>
        [Serialize]
        [InspectableIf(nameof(supportsChaining))]
        public bool chainable { get; set; }

        [DoNotSerialize]
        public bool supportsChaining => member.requiresTarget;

        [DoNotSerialize]
        [MemberFilter(Fields = true, Properties = true, ReadOnly = false)]
        public Member setter
        {
            get => member;
            set => member = value;
        }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput assign { get; private set; }

        [DoNotSerialize]
        [PortLabel("Value")]
        [PortLabelHidden]
        public ValueInput input { get; private set; }

        [DoNotSerialize]
        [PortLabel("Value")]
        [PortLabelHidden]
        public ValueOutput output { get; private set; }

        /// <summary>
        /// The target object used when setting the value.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Target")]
        [PortLabelHidden]
        public ValueOutput targetOutput { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput assigned { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            assign = ControlInput(nameof(assign), Assign);
            assigned = ControlOutput(nameof(assigned));
            Succession(assign, assigned);

            if (supportsChaining && chainable)
            {
                targetOutput = ValueOutput(member.targetType, nameof(targetOutput));
                Assignment(assign, targetOutput);
            }

            output = ValueOutput(member.type, nameof(output));
            Assignment(assign, output);

            if (member.requiresTarget)
            {
                Requirement(target, assign);
            }

            input = ValueInput(member.type, nameof(input));
            Requirement(input, assign);

            if (member.allowsNull)
            {
                input.AllowsNull();
            }

            input.SetDefaultValue(member.type.PseudoDefault());
        }

        protected override bool IsMemberValid(Member member)
        {
            return member.isAccessor && member.isSettable;
        }

        private object GetAndChainTarget(Flow flow)
        {
            if (member.requiresTarget)
            {
                var target = flow.GetValue(this.target, member.targetType);

                if (supportsChaining && chainable)
                {
                    flow.SetValue(targetOutput, target);
                }

                return target;
            }

            return null;
        }

        private ControlOutput Assign(Flow flow)
        {
            var target = GetAndChainTarget(flow);

            var value = flow.GetConvertedValue(input);

            flow.SetValue(output, member.Set(target, value));

            return assigned;
        }

        #region Analytics

        public override AnalyticsIdentifier GetAnalyticsIdentifier()
        {
            var aid = new AnalyticsIdentifier
            {
                Identifier = $"{member.targetType.FullName}.{member.name}(Set)",
                Namespace = member.targetType.Namespace,
            };
            aid.Hashcode = aid.Identifier.GetHashCode();
            return aid;
        }

        #endregion
    }
}
