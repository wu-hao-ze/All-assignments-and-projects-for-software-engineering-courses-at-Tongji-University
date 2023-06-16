using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the current game object.
    /// </summary>
    [SpecialUnit]
    [RenamedFrom("Bolt.Self")]
    [RenamedFrom("Unity.VisualScripting.Self")]
    public sealed class This : Unit
    {
        /// <summary>
        /// The current game object.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [PortLabel("This")]
        public ValueOutput self { get; private set; }

        protected override void Definition()
        {
            self = ValueOutput(nameof(self), Result).PredictableIf(IsPredictable);
        }

        private GameObject Result(Flow flow)
        {
            return flow.stack.self;
        }

        private bool IsPredictable(Flow flow)
        {
            return flow.stack.self != null;
        }
    }
}
