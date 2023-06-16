namespace Unity.VisualScripting
{
    public interface IStateDebugData : IGraphElementDebugData
    {
        int lastEnterFrame { get; }

        float lastExitTime { get; }
    }
}
