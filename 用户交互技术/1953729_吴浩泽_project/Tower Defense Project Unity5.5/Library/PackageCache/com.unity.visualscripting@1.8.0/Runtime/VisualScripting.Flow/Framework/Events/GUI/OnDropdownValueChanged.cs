using System;
using UnityEngine.UI;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the current value of the dropdown has changed.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(Dropdown))]
    [UnitOrder(4)]
    public sealed class OnDropdownValueChanged : GameObjectEventUnit<int>
    {
        public override Type MessageListenerType => typeof(UnityOnDropdownValueChangedMessageListener);
        protected override string hookName => EventHooks.OnDropdownValueChanged;

        /// <summary>
        /// The index of the newly selected option.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput index { get; private set; }

        /// <summary>
        /// The text of the newly selected option.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput text { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            index = ValueOutput<int>(nameof(index));
            text = ValueOutput<string>(nameof(text));
        }

        protected override void AssignArguments(Flow flow, int index)
        {
            flow.SetValue(this.index, index);
            flow.SetValue(text, flow.GetValue<Dropdown>(target).options[index].text);
        }
    }
}
