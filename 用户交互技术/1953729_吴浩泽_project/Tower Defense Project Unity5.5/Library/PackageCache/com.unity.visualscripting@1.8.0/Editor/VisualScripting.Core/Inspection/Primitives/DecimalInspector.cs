namespace Unity.VisualScripting
{
    [Inspector(typeof(decimal))]
    public class DecimalInspector : ContinuousNumberInspector<decimal>
    {
        public DecimalInspector(Metadata metadata) : base(metadata) { }
    }
}
