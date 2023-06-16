namespace Unity.VisualScripting
{
    [Inspector(typeof(byte))]
    public class ByteInspector : DiscreteNumberInspector<byte>
    {
        public ByteInspector(Metadata metadata) : base(metadata) { }
    }
}
