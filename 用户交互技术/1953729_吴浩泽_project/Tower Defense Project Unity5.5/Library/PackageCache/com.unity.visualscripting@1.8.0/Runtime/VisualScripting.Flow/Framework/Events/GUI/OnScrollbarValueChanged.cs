using System;
using UnityEngine.UI;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the current value of the scrollbar has changed.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(Scrollbar))]
    [UnitOrder(6)]
    public sealed class OnScrollbarValueChanged : GameObjectEventUnit<float>
    {
        public override Type MessageListenerType => typeof(UnityOnScrollbarValueChangedMessageListener);
        protected override string hookName => EventHooks.OnScrollbarValueChanged;

        /// <summary>
        /// The new position value of the scrollbar.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            value = ValueOutput<float>(nameof(value));
        }

        protected override void AssignArguments(Flow flow, float value)
        {
            flow.SetValue(this.value, value);
        }
    }
}
