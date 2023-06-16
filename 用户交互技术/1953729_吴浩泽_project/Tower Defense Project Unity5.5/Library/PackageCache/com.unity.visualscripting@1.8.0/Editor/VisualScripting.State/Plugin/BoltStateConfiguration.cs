namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    public sealed class BoltStateConfiguration : PluginConfiguration
    {
        private BoltStateConfiguration(BoltState plugin) : base(plugin) { }

        public override string header => "State Graphs";

        /// <summary>
        /// Determines under which condition events should be shown in state nodes.
        /// </summary>
        [EditorPref]
        public StateRevealCondition statesReveal { get; set; } = StateRevealCondition.Always;

        /// <summary>
        /// Determines under which condition event names should be shown in state transition.
        /// </summary>
        [EditorPref]
        public StateRevealCondition transitionsReveal { get; set; } = StateRevealCondition.OnHoverWithAlt;

        /// <summary>
        /// Whether state transitions should show an arrow at their destination state. This can appear confusing when there are
        /// multiple transitions.
        /// </summary>
        [EditorPref]
        public bool transitionsEndArrow { get; set; } = false;

        /// <summary>
        /// Whether traversed transitions should show a droplet animation.
        /// </summary>
        [EditorPref]
        public bool animateTransitions { get; set; } = true;
    }
}
