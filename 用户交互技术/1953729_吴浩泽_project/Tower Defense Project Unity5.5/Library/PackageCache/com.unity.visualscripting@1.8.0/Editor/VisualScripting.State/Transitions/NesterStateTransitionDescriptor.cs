namespace Unity.VisualScripting
{
    [Descriptor(typeof(INesterStateTransition))]
    public class NesterStateTransitionDescriptor<TNesterStateTransition> : StateTransitionDescriptor<TNesterStateTransition>
        where TNesterStateTransition : class, INesterStateTransition
    {
        public NesterStateTransitionDescriptor(TNesterStateTransition transition) : base(transition) { }

        [RequiresUnityAPI]
        public override string Title()
        {
            return GraphNesterDescriptor.Title(transition);
        }

        [RequiresUnityAPI]
        public override string Summary()
        {
            return GraphNesterDescriptor.Summary(transition);
        }
    }
}
