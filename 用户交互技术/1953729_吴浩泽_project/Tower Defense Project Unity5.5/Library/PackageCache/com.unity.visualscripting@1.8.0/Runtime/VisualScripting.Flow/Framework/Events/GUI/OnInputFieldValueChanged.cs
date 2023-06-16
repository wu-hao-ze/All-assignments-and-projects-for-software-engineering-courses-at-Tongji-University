using System;
using UnityEngine.UI;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the text content of the input field has changed.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(InputField))]
    [UnitOrder(2)]
    public sealed class OnInputFieldValueChanged : GameObjectEventUnit<string>
    {
        public override Type MessageListenerType => typeof(UnityOnInputFieldValueChangedMessageListener);
        protected override string hookName => EventHooks.OnInputFieldValueChanged;

        /// <summary>
        /// The new text content of the input field.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            value = ValueOutput<string>(nameof(value));
        }

        protected override void AssignArguments(Flow flow, string value)
        {
            flow.SetValue(this.value, value);
        }
    }
}
