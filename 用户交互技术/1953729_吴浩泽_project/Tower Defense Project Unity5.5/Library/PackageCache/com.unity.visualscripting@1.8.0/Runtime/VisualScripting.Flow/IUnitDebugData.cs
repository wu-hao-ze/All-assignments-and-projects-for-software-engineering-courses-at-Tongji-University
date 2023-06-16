namespace Unity.VisualScripting
{
    public interface IUnitDebugData : IGraphElementDebugData
    {
        int lastInvokeFrame { get; set; }

        float lastInvokeTime { get; set; }
    }
}
