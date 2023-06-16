#pragma warning disable 675

namespace Unity.VisualScripting
{
    public class OrHandler : BinaryOperatorHandler
    {
        public OrHandler() : base("Or", "Or", "|", "op_BitwiseOr")
        {
            Handle<bool, bool>((a, b) => a | b);

            Handle<byte, byte>((a, b) => a | b);
            Handle<byte, sbyte>((a, b) => a | b);
            Handle<byte, short>((a, b) => a | b);
            Handle<byte, ushort>((a, b) => a | b);
            Handle<byte, int>((a, b) => a | b);
            Handle<byte, uint>((a, b) => a | b);
            Handle<byte, long>((a, b) => a | b);
            Handle<byte, ulong>((a, b) => a | b);

            Handle<sbyte, byte>((a, b) => a | b);
            Handle<sbyte, sbyte>((a, b) => a | b);
            Handle<sbyte, short>((a, b) => a | b);
            Handle<sbyte, ushort>((a, b) => a | b);
            Handle<sbyte, int>((a, b) => a | b);
            Handle<sbyte, uint>((a, b) => a | b);
            Handle<sbyte, long>((a, b) => a | b);
            //Handle<sbyte, ulong>((a, b) => a | b);

            Handle<short, byte>((a, b) => a | b);
            Handle<short, sbyte>((a, b) => a | b);
            Handle<short, short>((a, b) => a | b);
            Handle<short, ushort>((a, b) => a | b);
            Handle<short, int>((a, b) => a | b);
            Handle<short, uint>((a, b) => a | b);
            Handle<short, long>((a, b) => a | b);
            //Handle<short, ulong>((a, b) => a | b);

            Handle<ushort, byte>((a, b) => a | b);
            Handle<ushort, sbyte>((a, b) => a | b);
            Handle<ushort, short>((a, b) => a | b);
            Handle<ushort, ushort>((a, b) => a | b);
            Handle<ushort, int>((a, b) => a | b);
            Handle<ushort, uint>((a, b) => a | b);
            Handle<ushort, long>((a, b) => a | b);
            Handle<ushort, ulong>((a, b) => a | b);

            Handle<int, byte>((a, b) => a | b);
            Handle<int, sbyte>((a, b) => a | b);
            Handle<int, short>((a, b) => a | b);
            Handle<int, ushort>((a, b) => a | b);
            Handle<int, int>((a, b) => a | b);
            Handle<int, uint>((a, b) => a | b);
            Handle<int, long>((a, b) => a | b);
            //Handle<int, ulong>((a, b) => a | b);

            Handle<uint, byte>((a, b) => a | b);
            Handle<uint, sbyte>((a, b) => a | b);
            Handle<uint, short>((a, b) => a | b);
            Handle<uint, ushort>((a, b) => a | b);
            Handle<uint, int>((a, b) => a | b);
            Handle<uint, uint>((a, b) => a | b);
            Handle<uint, long>((a, b) => a | b);
            Handle<uint, ulong>((a, b) => a | b);

            Handle<long, byte>((a, b) => a | b);
            Handle<long, sbyte>((a, b) => a | b);
            Handle<long, short>((a, b) => a | b);
            Handle<long, ushort>((a, b) => a | b);
            Handle<long, int>((a, b) => a | b);
            Handle<long, uint>((a, b) => a | b);
            Handle<long, long>((a, b) => a | b);
            //Handle<long, ulong>((a, b) => a | b);

            Handle<ulong, byte>((a, b) => a | b);
            //Handle<ulong, sbyte>((a, b) => a | b);
            //Handle<ulong, short>((a, b) => a | b);
            Handle<ulong, ushort>((a, b) => a | b);
            //Handle<ulong, int>((a, b) => a | b);
            Handle<ulong, uint>((a, b) => a | b);
            //Handle<ulong, long>((a, b) => a | b);
            Handle<ulong, ulong>((a, b) => a | b);
        }
    }
}
