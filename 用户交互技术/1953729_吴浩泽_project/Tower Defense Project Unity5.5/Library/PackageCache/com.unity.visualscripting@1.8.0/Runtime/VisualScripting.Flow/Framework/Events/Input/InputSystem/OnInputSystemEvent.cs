using System;
using Unity.VisualScripting;
using UnityEngine;
#if PACKAGE_INPUT_SYSTEM_EXISTS
using UnityEngine.InputSystem;

namespace Unity.VisualScripting.InputSystem
{
    public enum InputActionChangeOption
    {
        OnPressed,
        OnHold,
        OnReleased,
    }

    public enum OutputType
    {
        Button,
        Float,
        Vector2
    }

    /// <summary>
    /// A configurable event to handle input system events.
    /// </summary>
    [UnitCategory("Events/Input")]
    public abstract class OnInputSystemEvent : MachineEventUnit<EmptyEventArgs>
    {
        protected override string hookName =>
            UnityEngine.InputSystem.InputSystem.settings.updateMode ==
            InputSettings.UpdateMode.ProcessEventsInDynamicUpdate
            ? EventHooks.Update
            : EventHooks.FixedUpdate;

        protected abstract OutputType OutputType { get; }

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputActionChangeOption InputActionChangeType;

        /// <summary>
        /// The InputAction. If it's selected from the dropdown populated from the target PlayerInput, the widget will
        /// serialize a fake PlayerAction with the right ID/name, but it will need to be compared to the actual action
        /// fetched at runtime from the playerInput's action asset as the fake one won't trigger any event
        /// </summary>
        [DoNotSerialize]
        public ValueInput InputAction { get; private set; }

        /// <summary>
        /// The target PlayerInput component. if null, the InputAction is expected to be fetched dynamically
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput Target { get; private set; }

        [PortLabelHidden]
        public ValueOutput FloatValue { get; private set; }
        [PortLabelHidden]
        public ValueOutput Vector2Value { get; private set; }

        private InputAction m_Action;

#if !PACKAGE_INPUT_SYSTEM_1_2_0_OR_NEWER_EXISTS
        private bool m_WasRunning;
#endif


        /// <summary>
        ///  Stores the last value, and returned by the output port. This intermediary value is there to enable
        /// "fetching" from the value port, which happens when the value is read, but the node has never been executed.
        /// Typical use case is "on some other event, read the value of the axis/joystick even if it was never used"
        /// </summary>
        private Vector2 m_Value;

        protected override void Definition()
        {
            base.Definition();

            Target = ValueInput(typeof(PlayerInput), nameof(Target));
            Target.SetDefaultValue(null);
            Target.NullMeansSelf();

            InputAction = ValueInput(typeof(InputAction), nameof(InputAction));
            InputAction.SetDefaultValue(default(InputAction));

            switch (OutputType)
            {
                case OutputType.Button:
                    break;
                case OutputType.Float:
                    // the getValue delegate is what enables fetching
                    FloatValue = ValueOutput<float>(nameof(FloatValue), _ => m_Value.x);
                    break;
                case OutputType.Vector2:
                    // the getValue delegate is what enables fetching
                    Vector2Value = ValueOutput<Vector2>(nameof(Vector2Value), _ => m_Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);
            var graphReference = stack.ToReference();

            var pi = Flow.FetchValue<PlayerInput>(Target, graphReference);
            var inputAction = Flow.FetchValue<InputAction>(InputAction, graphReference);
            if (inputAction == null)
                return;
            m_Action = pi
                ? pi.actions.FindAction(inputAction.id)
                : inputAction.actionMap != null ? inputAction : null;
        }

        public override void StopListening(GraphStack stack)
        {
            base.StopListening(stack);
            m_Action = null;
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            if (m_Action == null)
                return false;

            bool shouldTrigger;

#if PACKAGE_INPUT_SYSTEM_1_2_0_OR_NEWER_EXISTS
            switch (InputActionChangeType)
            {
                case InputActionChangeOption.OnPressed:
                    shouldTrigger = m_Action.WasPressedThisFrame();
                    break;
                case InputActionChangeOption.OnHold:
                    shouldTrigger = m_Action.IsPressed();
                    break;
                case InputActionChangeOption.OnReleased:
                    shouldTrigger = m_Action.WasReleasedThisFrame();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#else
            // "Started" is true while the button is held, triggered is true only one frame. hence what looks like a bug but isn't
            switch (InputActionChangeType)
            {
                case InputActionChangeOption.OnPressed:
                    shouldTrigger = m_Action.triggered; // started is true too long
                    break;
                case InputActionChangeOption.OnHold:
                    shouldTrigger = m_Action.phase == InputActionPhase.Started; // triggered is only true one frame
                    break;
                case InputActionChangeOption.OnReleased:
                    shouldTrigger = m_WasRunning && m_Action.phase != InputActionPhase.Started; // never equal to InputActionPhase.Cancelled when polling
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Hack - can't make sense of the action phase when polled (always Started or Waiting). Fallback on "== Started"
            m_WasRunning = m_Action.phase == InputActionPhase.Started;
#endif

            DoAssignArguments(flow);
            return shouldTrigger;
        }

        private void DoAssignArguments(Flow flow)
        {
            switch (OutputType)
            {
                case OutputType.Button:
                    break;
                case OutputType.Float:
                    var readValue = m_Action.ReadValue<float>();
                    m_Value.Set(readValue, 0);
                    flow.SetValue(FloatValue, readValue);
                    break;
                case OutputType.Vector2:
                    var vector2 = m_Action.ReadValue<Vector2>();
                    m_Value = vector2;
                    flow.SetValue(Vector2Value, vector2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class OnInputSystemEventButton : OnInputSystemEvent
    {
        protected override OutputType OutputType => OutputType.Button;
    }

    public class OnInputSystemEventFloat : OnInputSystemEvent
    {
        protected override OutputType OutputType => OutputType.Float;
    }

    public class OnInputSystemEventVector2 : OnInputSystemEvent
    {
        protected override OutputType OutputType => OutputType.Vector2;
    }
}
#endif
