namespace Unity.VisualScripting
{
    [Inspector(typeof(float))]
    public class FloatInspector : ContinuousNumberInspector<float>
    {
        public FloatInspector(Metadata metadata) : base(metadata) { }
    }
}
