namespace Unity.VisualScripting
{
    public abstract class StateTransitionDescriptor<TStateTransition> : Descriptor<TStateTransition, StateTransitionDescription>
        where TStateTransition : class, IStateTransition
    {
        protected StateTransitionDescriptor(TStateTransition target) : base(target) { }

        public TStateTransition transition => target;

        [Assigns]
        public override string Title()
        {
            return "Transition";
        }

        [Assigns]
        public override string Summary()
        {
            return null;
        }

        [Assigns]
        public virtual string Label()
        {
            return Title();
        }

        [Assigns]
        public virtual string Tooltip()
        {
            return Summary();
        }

        [Assigns]
        [RequiresUnityAPI]
        public override EditorTexture Icon()
        {
            return typeof(IStateTransition).Icon();
        }
    }
}
