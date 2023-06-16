using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// A configurable event to handle global keyboard input.
    /// </summary>
    [UnitCategory("Events/Input")]
    public sealed class OnKeyboardInput : MachineEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.Update;

        /// <summary>
        /// The key that received input.
        /// </summary>
        [DoNotSerialize]
        public ValueInput key { get; private set; }

        /// <summary>
        /// The type of input.
        /// </summary>
        [DoNotSerialize]
        public ValueInput action { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            key = ValueInput(nameof(key), KeyCode.Space);
            action = ValueInput(nameof(action), PressState.Down);
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            var key = flow.GetValue<KeyCode>(this.key);
            var action = flow.GetValue<PressState>(this.action);

            switch (action)
            {
                case PressState.Down: return Input.GetKeyDown(key);
                case PressState.Up: return Input.GetKeyUp(key);
                case PressState.Hold: return Input.GetKey(key);
                default: throw new UnexpectedEnumValueException<PressState>(action);
            }
        }
    }
}
