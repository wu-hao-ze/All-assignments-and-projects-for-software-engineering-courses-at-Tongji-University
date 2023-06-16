namespace Unity.VisualScripting
{
    public sealed class LogicalNegationHandler : UnaryOperatorHandler
    {
        public LogicalNegationHandler() : base("Logical Negation", "Not", "~", "op_OnesComplement")
        {
            Handle<bool>(a => !a);
            Handle<byte>(a => ~a);
            Handle<sbyte>(a => ~a);
            Handle<short>(a => ~a);
            Handle<ushort>(a => ~a);
            Handle<int>(a => ~a);
            Handle<uint>(a => ~a);
            Handle<long>(a => ~a);
            Handle<ulong>(a => ~a);
        }
    }
}
