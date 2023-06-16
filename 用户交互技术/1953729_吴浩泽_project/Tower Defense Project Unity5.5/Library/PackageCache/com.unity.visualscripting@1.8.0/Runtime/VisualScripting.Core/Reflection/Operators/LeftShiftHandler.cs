namespace Unity.VisualScripting
{
    public class LeftShiftHandler : BinaryOperatorHandler
    {
        public LeftShiftHandler() : base("Left Shift", "Left Shift", "<<", "op_LeftShift")
        {
            Handle<byte, byte>((a, b) => a << b);
            Handle<byte, sbyte>((a, b) => a << b);
            Handle<byte, short>((a, b) => a << b);
            Handle<byte, ushort>((a, b) => a << b);
            Handle<byte, int>((a, b) => a << b);

            Handle<sbyte, byte>((a, b) => a << b);
            Handle<sbyte, sbyte>((a, b) => a << b);
            Handle<sbyte, short>((a, b) => a << b);
            Handle<sbyte, ushort>((a, b) => a << b);
            Handle<sbyte, int>((a, b) => a << b);

            Handle<short, byte>((a, b) => a << b);
            Handle<short, sbyte>((a, b) => a << b);
            Handle<short, short>((a, b) => a << b);
            Handle<short, ushort>((a, b) => a << b);
            Handle<short, int>((a, b) => a << b);

            Handle<ushort, byte>((a, b) => a << b);
            Handle<ushort, sbyte>((a, b) => a << b);
            Handle<ushort, short>((a, b) => a << b);
            Handle<ushort, ushort>((a, b) => a << b);
            Handle<ushort, int>((a, b) => a << b);

            Handle<int, byte>((a, b) => a << b);
            Handle<int, sbyte>((a, b) => a << b);
            Handle<int, short>((a, b) => a << b);
            Handle<int, ushort>((a, b) => a << b);
            Handle<int, int>((a, b) => a << b);

            Handle<uint, byte>((a, b) => a << b);
            Handle<uint, sbyte>((a, b) => a << b);
            Handle<uint, short>((a, b) => a << b);
            Handle<uint, ushort>((a, b) => a << b);
            Handle<uint, int>((a, b) => a << b);

            Handle<long, byte>((a, b) => a << b);
            Handle<long, sbyte>((a, b) => a << b);
            Handle<long, short>((a, b) => a << b);
            Handle<long, ushort>((a, b) => a << b);
            Handle<long, int>((a, b) => a << b);

            Handle<ulong, byte>((a, b) => a << b);
            Handle<ulong, sbyte>((a, b) => a << b);
            Handle<ulong, short>((a, b) => a << b);
            Handle<ulong, ushort>((a, b) => a << b);
            Handle<ulong, int>((a, b) => a << b);
        }
    }
}
