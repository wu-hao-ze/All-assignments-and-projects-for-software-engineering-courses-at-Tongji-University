namespace Unity.VisualScripting
{
    [Inspector(typeof(uint))]
    public class UintInspector : DiscreteNumberInspector<uint>
    {
        public UintInspector(Metadata metadata) : base(metadata) { }
    }
}
