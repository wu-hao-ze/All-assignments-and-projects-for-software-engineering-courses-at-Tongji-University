using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Throws an exception.
    /// </summary>
    [UnitCategory("Control")]
    [UnitOrder(16)]
    public sealed class Throw : Unit
    {
        /// <summary>
        /// Whether a custom exception object should be specified manually.
        /// </summary>
        [Serialize]
        [Inspectable, UnitHeaderInspectable("Custom")]
        [InspectorToggleLeft]
        public bool custom { get; set; }

        /// <summary>
        /// The entry point to throw the exception.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The message of the exception.
        /// </summary>
        [DoNotSerialize]
        public ValueInput message { get; private set; }

        /// <summary>
        /// The exception to throw.
        /// </summary>
        [DoNotSerialize]
        public ValueInput exception { get; private set; }

        protected override void Definition()
        {
            if (custom)
            {
                enter = ControlInput(nameof(enter), ThrowCustom);
                exception = ValueInput<Exception>(nameof(exception));
                Requirement(exception, enter);
            }
            else
            {
                enter = ControlInput(nameof(enter), ThrowMessage);
                message = ValueInput(nameof(message), string.Empty);
                Requirement(message, enter);
            }
        }

        private ControlOutput ThrowCustom(Flow flow)
        {
            throw flow.GetValue<Exception>(exception);
        }

        private ControlOutput ThrowMessage(Flow flow)
        {
            throw new Exception(flow.GetValue<string>(message));
        }
    }
}
