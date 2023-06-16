namespace Unity.VisualScripting
{
    [TypeIcon(typeof(StateGraph))]
    [UnitCategory("Nesting")]
    public sealed class StateUnit : NesterUnit<StateGraph, StateGraphAsset>
    {
        public StateUnit() : base() { }

        public StateUnit(StateGraphAsset macro) : base(macro) { }

        /// <summary>
        /// The entry point to start the state graph.
        /// </summary>
        [DoNotSerialize]
        public ControlInput start { get; private set; }

        /// <summary>
        /// The entry point to stop the state graph.
        /// </summary>
        [DoNotSerialize]
        public ControlInput stop { get; private set; }

        /// <summary>
        /// The action to execute after the state graph has been started.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput started { get; private set; }

        /// <summary>
        /// The action to execute after the state graph has been stopped.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput stopped { get; private set; }

        public static StateUnit WithStart()
        {
            var stateUnit = new StateUnit();
            stateUnit.nest.source = GraphSource.Embed;
            stateUnit.nest.embed = StateGraph.WithStart();
            return stateUnit;
        }

        protected override void Definition()
        {
            start = ControlInput(nameof(start), Start);
            stop = ControlInput(nameof(stop), Stop);

            started = ControlOutput(nameof(started));
            stopped = ControlOutput(nameof(stopped));

            Succession(start, started);
            Succession(stop, stopped);
        }

        private ControlOutput Start(Flow flow)
        {
            flow.stack.EnterParentElement(this);
            nest.graph.Start(flow);
            flow.stack.ExitParentElement();
            return started;
        }

        private ControlOutput Stop(Flow flow)
        {
            flow.stack.EnterParentElement(this);
            nest.graph.Stop(flow);
            flow.stack.ExitParentElement();
            return stopped;
        }

        public override StateGraph DefaultGraph()
        {
            return StateGraph.WithStart();
        }
    }
}
