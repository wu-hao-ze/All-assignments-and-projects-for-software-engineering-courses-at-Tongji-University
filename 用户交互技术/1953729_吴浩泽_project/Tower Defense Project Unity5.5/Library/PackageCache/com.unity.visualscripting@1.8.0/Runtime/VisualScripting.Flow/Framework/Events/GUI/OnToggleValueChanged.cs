using System;
using UnityEngine.UI;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the current value of the toggle has changed.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(Toggle))]
    [UnitOrder(5)]
    public sealed class OnToggleValueChanged : GameObjectEventUnit<bool>
    {
        public override Type MessageListenerType => typeof(UnityOnToggleValueChangedMessageListener);
        protected override string hookName => EventHooks.OnToggleValueChanged;

        /// <summary>
        /// The new boolean value of the toggle.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            value = ValueOutput<bool>(nameof(value));
        }

        protected override void AssignArguments(Flow flow, bool value)
        {
            flow.SetValue(this.value, value);
        }
    }
}
