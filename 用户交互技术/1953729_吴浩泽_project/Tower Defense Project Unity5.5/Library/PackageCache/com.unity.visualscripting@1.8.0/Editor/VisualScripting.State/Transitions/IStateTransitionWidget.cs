namespace Unity.VisualScripting
{
    public interface IStateTransitionWidget : INodeWidget
    {
        Edge sourceEdge { get; }
    }
}
