namespace Unity.VisualScripting
{
    /// <summary>
    /// Triggers the transition in the parent state graph.
    /// </summary>
    [UnitSurtitle("State")]
    [UnitCategory("Nesting")]
    [UnitShortTitle("Trigger Transition")]
    [TypeIcon(typeof(IStateTransition))]
    public sealed class TriggerStateTransition : Unit
    {
        /// <summary>
        /// The moment at which the parent state transition should be triggered.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput trigger { get; private set; }

        protected override void Definition()
        {
            trigger = ControlInput(nameof(trigger), Trigger);
        }

        private ControlOutput Trigger(Flow flow)
        {
            var stateTransition = flow.stack.GetParent<INesterStateTransition>();

            flow.stack.ExitParentElement();

            stateTransition.Branch(flow);

            return null;
        }
    }
}
