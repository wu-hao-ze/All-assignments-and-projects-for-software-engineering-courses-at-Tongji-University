namespace Unity.VisualScripting
{
    public class GraphItemDescriptor<TItem, TDescription> : Descriptor<TItem, TDescription>
        where TItem : class, IGraphItem
        where TDescription : class, IDescription, new()
    {
        protected GraphItemDescriptor(TItem item) : base(item) { }
    }
}
