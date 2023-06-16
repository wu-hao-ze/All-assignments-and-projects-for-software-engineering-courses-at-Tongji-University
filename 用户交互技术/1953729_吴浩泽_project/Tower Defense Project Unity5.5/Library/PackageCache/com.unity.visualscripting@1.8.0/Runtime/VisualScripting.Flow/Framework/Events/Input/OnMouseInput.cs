using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// A configurable event to handle global mouse input.
    /// </summary>
    [UnitCategory("Events/Input")]
    public sealed class OnMouseInput : MachineEventUnit<EmptyEventArgs>, IMouseEventUnit
    {
        protected override string hookName => EventHooks.Update;

        /// <summary>
        /// The mouse button that received input.
        /// </summary>
        [DoNotSerialize]
        public ValueInput button { get; private set; }

        /// <summary>
        /// The type of input.
        /// </summary>
        [DoNotSerialize]
        public ValueInput action { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            button = ValueInput(nameof(button), MouseButton.Left);
            action = ValueInput(nameof(action), PressState.Down);
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            var button = (int)flow.GetValue<MouseButton>(this.button);
            var action = flow.GetValue<PressState>(this.action);

            switch (action)
            {
                case PressState.Down: return Input.GetMouseButtonDown(button);
                case PressState.Up: return Input.GetMouseButtonUp(button);
                case PressState.Hold: return Input.GetMouseButton(button);
                default: throw new UnexpectedEnumValueException<PressState>(action);
            }
        }
    }
}
