namespace Unity.VisualScripting
{
    public interface IStateTransitionDebugData : IGraphElementDebugData
    {
        int lastBranchFrame { get; }

        float lastBranchTime { get; }
    }
}
