namespace Unity.VisualScripting
{
    public interface IStateWidget : IGraphElementWidget
    {
        IState state { get; }
    }
}
