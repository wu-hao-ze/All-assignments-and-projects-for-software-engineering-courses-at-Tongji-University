namespace Unity.VisualScripting
{
    [Inspector(typeof(sbyte))]
    public class SbyteInspector : DiscreteNumberInspector<sbyte>
    {
        public SbyteInspector(Metadata metadata) : base(metadata) { }
    }
}
