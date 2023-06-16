namespace Unity.VisualScripting
{
    /// <summary>
    /// Branches flow by checking if a condition is true or false.
    /// </summary>
    [UnitCategory("Control")]
    [UnitOrder(0)]
    [RenamedFrom("Bolt.Branch")]
    [RenamedFrom("Unity.VisualScripting.Branch")]
    public sealed class If : Unit, IBranchUnit
    {
        /// <summary>
        /// The entry point for the branch.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The condition to check.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput condition { get; private set; }

        /// <summary>
        /// The action to execute if the condition is true.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("True")]
        public ControlOutput ifTrue { get; private set; }

        /// <summary>
        /// The action to execute if the condition is false.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("False")]
        public ControlOutput ifFalse { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);
            condition = ValueInput<bool>(nameof(condition));
            ifTrue = ControlOutput(nameof(ifTrue));
            ifFalse = ControlOutput(nameof(ifFalse));

            Requirement(condition, enter);
            Succession(enter, ifTrue);
            Succession(enter, ifFalse);
        }

        public ControlOutput Enter(Flow flow)
        {
            return flow.GetValue<bool>(condition) ? ifTrue : ifFalse;
        }
    }
}
