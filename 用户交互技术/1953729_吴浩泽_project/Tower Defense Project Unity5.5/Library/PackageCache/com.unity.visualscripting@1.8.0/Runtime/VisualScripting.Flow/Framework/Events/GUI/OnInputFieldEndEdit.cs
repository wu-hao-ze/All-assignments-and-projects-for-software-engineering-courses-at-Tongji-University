using System;
using UnityEngine.UI;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the user finishes editing the text content either by submitting or by clicking somewhere that removes the
    /// focus from the input field.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(InputField))]
    [UnitOrder(3)]
    public sealed class OnInputFieldEndEdit : GameObjectEventUnit<string>
    {
        public override Type MessageListenerType => typeof(UnityOnInputFieldEndEditMessageListener);
        protected override string hookName => EventHooks.OnInputFieldEndEdit;

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
