using System;
using UnityEngine.UI;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the current value of the slider has changed.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(Slider))]
    [UnitOrder(8)]
    public sealed class OnSliderValueChanged : GameObjectEventUnit<float>
    {
        public override Type MessageListenerType => typeof(UnityOnSliderValueChangedMessageListener);
        protected override string hookName => EventHooks.OnSliderValueChanged;

        /// <summary>
        /// The new numeric value of the slider.
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
