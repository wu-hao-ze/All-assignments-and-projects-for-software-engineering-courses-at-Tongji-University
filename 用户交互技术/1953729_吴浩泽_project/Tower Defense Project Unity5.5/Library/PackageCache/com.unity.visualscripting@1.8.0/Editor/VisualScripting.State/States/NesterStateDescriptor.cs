namespace Unity.VisualScripting
{
    [Descriptor(typeof(INesterState))]
    public class NesterStateDescriptor<TNesterState> : StateDescriptor<TNesterState>
        where TNesterState : class, INesterState
    {
        public NesterStateDescriptor(TNesterState state) : base(state) { }

        [RequiresUnityAPI]
        public override string Title()
        {
            return GraphNesterDescriptor.Title(state);
        }

        [RequiresUnityAPI]
        public override string Summary()
        {
            return GraphNesterDescriptor.Summary(state);
        }
    }
}
