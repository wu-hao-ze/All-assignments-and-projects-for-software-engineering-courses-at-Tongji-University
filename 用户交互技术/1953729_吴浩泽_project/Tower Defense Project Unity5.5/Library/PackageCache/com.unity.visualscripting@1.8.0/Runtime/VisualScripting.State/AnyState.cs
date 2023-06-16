namespace Unity.VisualScripting
{
    /// <summary>
    /// A special state that can trigger transitions to other states,
    /// no matter which state is currently active. This state cannot receive
    /// transitions.
    /// </summary>
    public sealed class AnyState : State
    {
        [DoNotSerialize]
        public override bool canBeDestination => false;

        public AnyState() : base()
        {
            isStart = true;
        }

        public override void OnExit(Flow flow, StateExitReason reason)
        {
            // Don't exit this state from branching.
            if (reason == StateExitReason.Branch)
            {
                return;
            }

            base.OnExit(flow, reason);
        }

        public override void OnBranchTo(Flow flow, IState destination)
        {
            // Before entering the destination destination state,
            // exit all other connected states.

            foreach (var outgoingTransition in outgoingTransitionsNoAlloc)
            {
                if (outgoingTransition.destination != destination)
                {
                    outgoingTransition.destination.OnExit(flow, StateExitReason.AnyBranch);
                }
            }
        }
    }
}
