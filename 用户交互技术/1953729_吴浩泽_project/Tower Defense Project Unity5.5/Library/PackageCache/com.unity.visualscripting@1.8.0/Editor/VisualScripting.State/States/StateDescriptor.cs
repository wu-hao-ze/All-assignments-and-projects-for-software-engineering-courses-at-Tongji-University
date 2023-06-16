namespace Unity.VisualScripting
{
    [Descriptor(typeof(IState))]
    public class StateDescriptor<TState> : Descriptor<TState, StateDescription>
        where TState : class, IState
    {
        public StateDescriptor(TState target) : base(target) { }

        public TState state => target;

        [Assigns]
        public override string Title()
        {
            return state.GetType().HumanName();
        }

        [Assigns]
        public override string Summary()
        {
            return state.GetType().Summary();
        }

        [Assigns]
        [RequiresUnityAPI]
        public override EditorTexture Icon()
        {
            return state.GetType().Icon();
        }
    }
}
