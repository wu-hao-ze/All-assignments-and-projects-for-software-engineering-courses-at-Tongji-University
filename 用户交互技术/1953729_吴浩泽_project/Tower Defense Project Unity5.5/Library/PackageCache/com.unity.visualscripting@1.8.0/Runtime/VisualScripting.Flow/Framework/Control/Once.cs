namespace Unity.VisualScripting
{
    /// <summary>
    /// Executes an action only once, and a different action afterwards.
    /// </summary>
    [UnitCategory("Control")]
    [UnitOrder(14)]
    public sealed class Once : Unit, IGraphElementWithData
    {
        public sealed class Data : IGraphElementData
        {
            public bool executed;
        }

        /// <summary>
        /// The entry point for the action.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// Trigger to reset the once check.
        /// </summary>
        [DoNotSerialize]
        public ControlInput reset { get; private set; }

        /// <summary>
        /// The action to execute the first time the node is entered.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput once { get; private set; }

        /// <summary>
        /// The action to execute subsequently.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput after { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);
            reset = ControlInput(nameof(reset), Reset);
            once = ControlOutput(nameof(once));
            after = ControlOutput(nameof(after));

            Succession(enter, once);
            Succession(enter, after);
        }

        public IGraphElementData CreateData()
        {
            return new Data();
        }

        public ControlOutput Enter(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            if (!data.executed)
            {
                data.executed = true;

                return once;
            }
            else
            {
                return after;
            }
        }

        public ControlOutput Reset(Flow flow)
        {
            flow.stack.GetElementData<Data>(this).executed = false;

            return null;
        }
    }
}
