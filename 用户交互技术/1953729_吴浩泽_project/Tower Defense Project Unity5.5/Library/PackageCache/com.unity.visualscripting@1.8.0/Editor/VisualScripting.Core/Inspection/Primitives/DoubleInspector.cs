namespace Unity.VisualScripting
{
    [Inspector(typeof(double))]
    public class DoubleInspector : ContinuousNumberInspector<double>
    {
        public DoubleInspector(Metadata metadata) : base(metadata) { }
    }
}
