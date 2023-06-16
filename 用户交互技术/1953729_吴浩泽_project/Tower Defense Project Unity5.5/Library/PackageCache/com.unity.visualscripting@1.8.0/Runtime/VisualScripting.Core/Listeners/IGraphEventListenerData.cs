namespace Unity.VisualScripting
{
    public interface IGraphEventListenerData : IGraphData
    {
        bool isListening { get; }
    }
}
