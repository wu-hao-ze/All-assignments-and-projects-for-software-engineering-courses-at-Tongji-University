namespace Unity.VisualScripting
{
    /// <summary>
    /// Stops the execution of the current loop.
    /// </summary>
    [UnitTitle("Break Loop")]
    [UnitCategory("Control")]
    [UnitOrder(13)]
    public class Break : Unit
    {
        /// <summary>
        /// The entry point for the break.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Operation);
        }

        public ControlOutput Operation(Flow flow)
        {
            flow.BreakLoop();

            return null;
        }
    }
}
