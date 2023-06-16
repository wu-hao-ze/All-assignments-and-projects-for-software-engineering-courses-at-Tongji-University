namespace Unity.VisualScripting
{
    public abstract class Descriptor<TTarget, TDescription> : Assigner<TTarget, TDescription>, IDescriptor
        where TTarget : class
        where TDescription : class, IDescription, new()
    {
        protected Descriptor(TTarget target) : base(target, new TDescription()) { }

        public override void ValueChanged()
        {
            DescriptorProvider.instance.TriggerDescriptionChange(target);
        }

        [Assigns]
        public virtual string Title()
        {
            return target.ToString();
        }

        [Assigns]
        public virtual string Summary()
        {
            return target.GetType().Summary();
        }

        [Assigns]
        [RequiresUnityAPI]
        public virtual EditorTexture Icon()
        {
            return target.GetType().Icon();
        }

        object IDescriptor.target => target;

        public TDescription description => assignee;

        IDescription IDescriptor.description => description;
    }
}
