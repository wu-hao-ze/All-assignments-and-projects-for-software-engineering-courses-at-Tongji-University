namespace Unity.VisualScripting
{
    /// <summary>
    /// Toggles between two values with on and off triggers.
    /// </summary>
    [UnitCategory("Control")]
    [UnitOrder(19)]
    [UnitFooterPorts(ControlInputs = true, ControlOutputs = true)]
    public sealed class ToggleValue : Unit, IGraphElementWithData
    {
        public class Data : IGraphElementData
        {
            public bool isOn;
        }

        /// <summary>
        /// Whether the toggle should start in the on state.
        /// </summary>
        [Serialize]
        [Inspectable]
        [UnitHeaderInspectable("Start On")]
        [InspectorToggleLeft]
        public bool startOn { get; set; } = true;

        /// <summary>
        /// Trigger to turn on the toggle.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("On")]
        public ControlInput turnOn { get; private set; }

        /// <summary>
        /// Trigger to turn off the toggle.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Off")]
        public ControlInput turnOff { get; private set; }

        /// <summary>
        /// Trigger to toggle the state of the toggle.
        /// </summary>
        [DoNotSerialize]
        public ControlInput toggle { get; private set; }

        /// <summary>
        /// The value to return if the toggle is on.
        /// </summary>
        [DoNotSerialize]
        public ValueInput onValue { get; private set; }

        /// <summary>
        /// The value to return if the toggle is off.
        /// </summary>
        [DoNotSerialize]
        public ValueInput offValue { get; private set; }

        /// <summary>
        /// Triggered when the flow gets turned on.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput turnedOn { get; private set; }

        /// <summary>
        /// Triggered when the flow gets turned off.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput turnedOff { get; private set; }

        /// <summary>
        /// Whether the flow is currently on.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput isOn { get; private set; }

        /// <summary>
        /// The value of the toggle selected depending on the state.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            turnOn = ControlInput(nameof(turnOn), TurnOn);
            turnOff = ControlInput(nameof(turnOff), TurnOff);
            toggle = ControlInput(nameof(toggle), Toggle);

            onValue = ValueInput<object>(nameof(onValue));
            offValue = ValueInput<object>(nameof(offValue));

            turnedOn = ControlOutput(nameof(turnedOn));
            turnedOff = ControlOutput(nameof(turnedOff));

            isOn = ValueOutput(nameof(isOn), IsOn);
            value = ValueOutput(nameof(value), Value);

            Requirement(onValue, value);
            Requirement(offValue, value);
            Succession(turnOn, turnedOn);
            Succession(turnOff, turnedOff);
            Succession(toggle, turnedOn);
            Succession(toggle, turnedOff);
        }

        public IGraphElementData CreateData()
        {
            return new Data() { isOn = startOn };
        }

        private bool IsOn(Flow flow)
        {
            return flow.stack.GetElementData<Data>(this).isOn;
        }

        private ControlOutput TurnOn(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            if (data.isOn)
            {
                return null;
            }

            data.isOn = true;

            return turnedOn;
        }

        private ControlOutput TurnOff(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            if (!data.isOn)
            {
                return null;
            }

            data.isOn = false;

            return turnedOff;
        }

        private ControlOutput Toggle(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            data.isOn = !data.isOn;

            return data.isOn ? turnedOn : turnedOff;
        }

        private object Value(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            return flow.GetValue(data.isOn ? onValue : offValue);
        }
    }
}
