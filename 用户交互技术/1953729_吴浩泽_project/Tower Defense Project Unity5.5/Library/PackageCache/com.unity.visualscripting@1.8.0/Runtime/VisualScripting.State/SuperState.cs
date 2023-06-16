namespace Unity.VisualScripting
{
    [TypeIcon(typeof(StateGraph))]
    public sealed class SuperState : NesterState<StateGraph, StateGraphAsset>, IGraphEventListener
    {
        public SuperState() : base() { }

        public SuperState(StateGraphAsset macro) : base(macro) { }

        public static SuperState WithStart()
        {
            var superState = new SuperState();
            superState.nest.source = GraphSource.Embed;
            superState.nest.embed = StateGraph.WithStart();
            return superState;
        }

        #region Lifecycle

        protected override void OnEnterImplementation(Flow flow)
        {
            if (flow.stack.TryEnterParentElement(this))
            {
                nest.graph.Start(flow);
                flow.stack.ExitParentElement();
            }
        }

        protected override void OnExitImplementation(Flow flow)
        {
            if (flow.stack.TryEnterParentElement(this))
            {
                nest.graph.Stop(flow);
                flow.stack.ExitParentElement();
            }
        }

        public void StartListening(GraphStack stack)
        {
            if (stack.TryEnterParentElement(this))
            {
                nest.graph.StartListening(stack);
                stack.ExitParentElement();
            }
        }

        public void StopListening(GraphStack stack)
        {
            if (stack.TryEnterParentElement(this))
            {
                nest.graph.StopListening(stack);
                stack.ExitParentElement();
            }
        }

        public bool IsListening(GraphPointer pointer)
        {
            return pointer.GetElementData<Data>(this).isActive;
        }

        #endregion


        public override StateGraph DefaultGraph()
        {
            return StateGraph.WithStart();
        }
    }
}
