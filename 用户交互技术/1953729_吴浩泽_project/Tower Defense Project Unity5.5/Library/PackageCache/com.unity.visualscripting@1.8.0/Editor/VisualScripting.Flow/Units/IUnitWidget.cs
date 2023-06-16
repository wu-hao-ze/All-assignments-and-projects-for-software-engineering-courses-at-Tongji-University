namespace Unity.VisualScripting
{
    public interface IUnitWidget : IGraphElementWidget
    {
        IUnit unit { get; }

        Inspector GetPortInspector(IUnitPort port, Metadata metadata);
    }
}
