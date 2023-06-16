using UnityEngine.AI;

namespace Unity.VisualScripting
{
#if MODULE_AI_EXISTS
    /// <summary>
    /// Called when the nav mesh agent comes within a certain threshold of its destination.
    /// </summary>
    [UnitCategory("Events/Navigation")]
    public sealed class OnDestinationReached : MachineEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.Update;

        /// <summary>
        /// The threshold for the remaining distance.
        /// </summary>
        [DoNotSerialize]
        public ValueInput threshold { get; private set; }

        /// <summary>
        /// Whether the event should only trigger when the path is not partial or invalid.
        /// </summary>
        [DoNotSerialize]
        public ValueInput requireSuccess { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            threshold = ValueInput(nameof(threshold), 0.05f);
            requireSuccess = ValueInput(nameof(requireSuccess), true);
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            var navMeshAgent = flow.stack.gameObject.GetComponent<NavMeshAgent>();

            return navMeshAgent != null &&
                navMeshAgent.remainingDistance <= flow.GetValue<float>(threshold) &&
                (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete || !flow.GetValue<bool>(requireSuccess));
        }
    }
#endif
}
