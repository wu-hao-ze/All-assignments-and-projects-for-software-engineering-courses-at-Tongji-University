using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// A configurable event to handle global button input.
    /// </summary>
    [UnitCategory("Events/Input")]
    public sealed class OnButtonInput : MachineEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.Update;

        /// <summary>
        /// The name of the button that received input.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Name")]
        public ValueInput buttonName { get; private set; }

        /// <summary>
        /// The type of input.
        /// </summary>
        [DoNotSerialize]
        public ValueInput action { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            buttonName = ValueInput(nameof(buttonName), string.Empty);
            action = ValueInput(nameof(action), PressState.Down);
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            var buttonName = flow.GetValue<string>(this.buttonName);
            var action = flow.GetValue<PressState>(this.action);

            switch (action)
            {
                case PressState.Down: return Input.GetButtonDown(buttonName);
                case PressState.Up: return Input.GetButtonUp(buttonName);
                case PressState.Hold: return Input.GetButton(buttonName);
                default: throw new UnexpectedEnumValueException<PressState>(action);
            }
        }
    }
}
