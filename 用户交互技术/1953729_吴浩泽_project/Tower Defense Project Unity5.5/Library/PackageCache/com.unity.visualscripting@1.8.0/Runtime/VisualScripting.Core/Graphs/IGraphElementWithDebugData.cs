namespace Unity.VisualScripting
{
    public interface IGraphElementWithDebugData : IGraphElement
    {
        IGraphElementDebugData CreateDebugData();
    }
}
