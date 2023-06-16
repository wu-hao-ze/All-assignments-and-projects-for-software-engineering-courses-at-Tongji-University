namespace Unity.VisualScripting
{
    public sealed class IncrementHandler : UnaryOperatorHandler
    {
        public IncrementHandler() : base("Increment", "Increment", "++", "op_Increment")
        {
            Handle<byte>(a => ++a);
            Handle<sbyte>(a => ++a);
            Handle<short>(a => ++a);
            Handle<ushort>(a => ++a);
            Handle<int>(a => ++a);
            Handle<uint>(a => ++a);
            Handle<long>(a => ++a);
            Handle<ulong>(a => ++a);
            Handle<float>(a => ++a);
            Handle<decimal>(a => ++a);
            Handle<double>(a => ++a);
        }
    }
}
