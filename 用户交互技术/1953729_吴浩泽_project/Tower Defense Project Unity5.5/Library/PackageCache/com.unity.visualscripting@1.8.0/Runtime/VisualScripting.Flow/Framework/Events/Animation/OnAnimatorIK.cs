using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called by the Animator Component immediately before it updates its internal IK system.
    /// This callback can be used to set the positions of the IK goals and their respective weights.
    /// </summary>
    [UnitCategory("Events/Animation")]
    public sealed class OnAnimatorIK : GameObjectEventUnit<int>
    {
        public override Type MessageListenerType => typeof(AnimatorMessageListener);
        protected override string hookName => EventHooks.OnAnimatorIK;

        /// <summary>
        /// The index of the layer on which the IK solver is called.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput layerIndex { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            layerIndex = ValueOutput<int>(nameof(layerIndex));
        }

        protected override void AssignArguments(Flow flow, int layerIndex)
        {
            flow.SetValue(this.layerIndex, layerIndex);
        }
    }
}
