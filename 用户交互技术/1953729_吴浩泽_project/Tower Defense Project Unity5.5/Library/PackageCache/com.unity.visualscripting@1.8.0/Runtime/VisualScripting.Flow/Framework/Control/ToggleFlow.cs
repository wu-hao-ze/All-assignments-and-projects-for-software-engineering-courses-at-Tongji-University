namespace Unity.VisualScripting
{
    /// <summary>
    /// Toggles the control flow with on and off triggers.
    /// </summary>
    [UnitCategory("Control")]
    [UnitOrder(18)]
    [UnitFooterPorts(ControlInputs = true, ControlOutputs = true)]
    public sealed class ToggleFlow : Unit, IGraphElementWithData
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
        /// Entry point to the toggle.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// Trigger to turn on the flow through the toggle.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("On")]
        public ControlInput turnOn { get; private set; }

        /// <summary>
        /// Trigger to turn off the flow through the toggle.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Off")]
        public ControlInput turnOff { get; private set; }

        /// <summary>
        /// Trigger to toggle the flow through the toggle.
        /// </summary>
        [DoNotSerialize]
        public ControlInput toggle { get; private set; }

        /// <summary>
        /// Triggered on entry if the flow is on.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("On")]
        public ControlOutput exitOn { get; private set; }

        /// <summary>
        /// Triggered on entry if the flow is off.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Off")]
        public ControlOutput exitOff { get; private set; }

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

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);
            turnOn = ControlInput(nameof(turnOn), TurnOn);
            turnOff = ControlInput(nameof(turnOff), TurnOff);
            toggle = ControlInput(nameof(toggle), Toggle);

            exitOn = ControlOutput(nameof(exitOn));
            exitOff = ControlOutput(nameof(exitOff));
            turnedOn = ControlOutput(nameof(turnedOn));
            turnedOff = ControlOutput(nameof(turnedOff));

            isOn = ValueOutput(nameof(isOn), IsOn);

            Succession(enter, exitOn);
            Succession(enter, exitOff);
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

        private ControlOutput Enter(Flow flow)
        {
            return IsOn(flow) ? exitOn : exitOff;
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
    }
}
