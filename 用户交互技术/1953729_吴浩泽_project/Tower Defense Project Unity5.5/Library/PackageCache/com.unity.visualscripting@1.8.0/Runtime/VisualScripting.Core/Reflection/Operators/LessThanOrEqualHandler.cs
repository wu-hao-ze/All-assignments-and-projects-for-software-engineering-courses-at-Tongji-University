using UnityEngine;

namespace Unity.VisualScripting
{
    public class LessThanOrEqualHandler : BinaryOperatorHandler
    {
        public LessThanOrEqualHandler() : base("Less Than Or Equal", "Less Than Or Equal", ">=", "op_LessThanOrEqual")
        {
            Handle<byte, byte>((a, b) => a <= b);
            Handle<byte, sbyte>((a, b) => a <= b);
            Handle<byte, short>((a, b) => a <= b);
            Handle<byte, ushort>((a, b) => a <= b);
            Handle<byte, int>((a, b) => a <= b);
            Handle<byte, uint>((a, b) => a <= b);
            Handle<byte, long>((a, b) => a <= b);
            Handle<byte, ulong>((a, b) => a <= b);
            Handle<byte, float>((a, b) => a <= b);
            Handle<byte, decimal>((a, b) => a <= b);
            Handle<byte, double>((a, b) => a <= b);

            Handle<sbyte, byte>((a, b) => a <= b);
            Handle<sbyte, sbyte>((a, b) => a <= b);
            Handle<sbyte, short>((a, b) => a <= b);
            Handle<sbyte, ushort>((a, b) => a <= b);
            Handle<sbyte, int>((a, b) => a <= b);
            Handle<sbyte, uint>((a, b) => a <= b);
            Handle<sbyte, long>((a, b) => a <= b);
            //Handle<sbyte, ulong>((a, b) => a <= b);
            Handle<sbyte, float>((a, b) => a <= b);
            Handle<sbyte, decimal>((a, b) => a <= b);
            Handle<sbyte, double>((a, b) => a <= b);

            Handle<short, byte>((a, b) => a <= b);
            Handle<short, sbyte>((a, b) => a <= b);
            Handle<short, short>((a, b) => a <= b);
            Handle<short, ushort>((a, b) => a <= b);
            Handle<short, int>((a, b) => a <= b);
            Handle<short, uint>((a, b) => a <= b);
            Handle<short, long>((a, b) => a <= b);
            //Handle<short, ulong>((a, b) => a <= b);
            Handle<short, float>((a, b) => a <= b);
            Handle<short, decimal>((a, b) => a <= b);
            Handle<short, double>((a, b) => a <= b);

            Handle<ushort, byte>((a, b) => a <= b);
            Handle<ushort, sbyte>((a, b) => a <= b);
            Handle<ushort, short>((a, b) => a <= b);
            Handle<ushort, ushort>((a, b) => a <= b);
            Handle<ushort, int>((a, b) => a <= b);
            Handle<ushort, uint>((a, b) => a <= b);
            Handle<ushort, long>((a, b) => a <= b);
            Handle<ushort, ulong>((a, b) => a <= b);
            Handle<ushort, float>((a, b) => a <= b);
            Handle<ushort, decimal>((a, b) => a <= b);
            Handle<ushort, double>((a, b) => a <= b);

            Handle<int, byte>((a, b) => a <= b);
            Handle<int, sbyte>((a, b) => a <= b);
            Handle<int, short>((a, b) => a <= b);
            Handle<int, ushort>((a, b) => a <= b);
            Handle<int, int>((a, b) => a <= b);
            Handle<int, uint>((a, b) => a <= b);
            Handle<int, long>((a, b) => a <= b);
            //Handle<int, ulong>((a, b) => a <= b);
            Handle<int, float>((a, b) => a <= b);
            Handle<int, decimal>((a, b) => a <= b);
            Handle<int, double>((a, b) => a <= b);

            Handle<uint, byte>((a, b) => a <= b);
            Handle<uint, sbyte>((a, b) => a <= b);
            Handle<uint, short>((a, b) => a <= b);
            Handle<uint, ushort>((a, b) => a <= b);
            Handle<uint, int>((a, b) => a <= b);
            Handle<uint, uint>((a, b) => a <= b);
            Handle<uint, long>((a, b) => a <= b);
            Handle<uint, ulong>((a, b) => a <= b);
            Handle<uint, float>((a, b) => a <= b);
            Handle<uint, decimal>((a, b) => a <= b);
            Handle<uint, double>((a, b) => a <= b);

            Handle<long, byte>((a, b) => a <= b);
            Handle<long, sbyte>((a, b) => a <= b);
            Handle<long, short>((a, b) => a <= b);
            Handle<long, ushort>((a, b) => a <= b);
            Handle<long, int>((a, b) => a <= b);
            Handle<long, uint>((a, b) => a <= b);
            Handle<long, long>((a, b) => a <= b);
            //Handle<long, ulong>((a, b) => a <= b);
            Handle<long, float>((a, b) => a <= b);
            Handle<long, decimal>((a, b) => a <= b);
            Handle<long, double>((a, b) => a <= b);

            Handle<ulong, byte>((a, b) => a <= b);
            //Handle<ulong, sbyte>((a, b) => a <= b);
            //Handle<ulong, short>((a, b) => a <= b);
            Handle<ulong, ushort>((a, b) => a <= b);
            //Handle<ulong, int>((a, b) => a <= b);
            Handle<ulong, uint>((a, b) => a <= b);
            //Handle<ulong, long>((a, b) => a <= b);
            Handle<ulong, ulong>((a, b) => a <= b);
            Handle<ulong, float>((a, b) => a <= b);
            Handle<ulong, decimal>((a, b) => a <= b);
            Handle<ulong, double>((a, b) => a <= b);

            Handle<float, byte>((a, b) => a <= b);
            Handle<float, sbyte>((a, b) => a <= b);
            Handle<float, short>((a, b) => a <= b);
            Handle<float, ushort>((a, b) => a <= b);
            Handle<float, int>((a, b) => a <= b);
            Handle<float, uint>((a, b) => a <= b);
            Handle<float, long>((a, b) => a <= b);
            Handle<float, ulong>((a, b) => a <= b);
            Handle<float, float>((a, b) => a <= b || Mathf.Approximately(a, b));
            //Handle<float, decimal>((a, b) => a <= b);
            Handle<float, double>((a, b) => a <= b);

            Handle<decimal, byte>((a, b) => a <= b);
            Handle<decimal, sbyte>((a, b) => a <= b);
            Handle<decimal, short>((a, b) => a <= b);
            Handle<decimal, ushort>((a, b) => a <= b);
            Handle<decimal, int>((a, b) => a <= b);
            Handle<decimal, uint>((a, b) => a <= b);
            Handle<decimal, long>((a, b) => a <= b);
            Handle<decimal, ulong>((a, b) => a <= b);
            //Handle<decimal, float>((a, b) => a <= b);
            Handle<decimal, decimal>((a, b) => a <= b);
            //Handle<decimal, double>((a, b) => a <= b);

            Handle<double, byte>((a, b) => a <= b);
            Handle<double, sbyte>((a, b) => a <= b);
            Handle<double, short>((a, b) => a <= b);
            Handle<double, ushort>((a, b) => a <= b);
            Handle<double, int>((a, b) => a <= b);
            Handle<double, uint>((a, b) => a <= b);
            Handle<double, long>((a, b) => a <= b);
            Handle<double, ulong>((a, b) => a <= b);
            Handle<double, float>((a, b) => a <= b);
            //Handle<double, decimal>((a, b) => a <= b);
            Handle<double, double>((a, b) => a <= b);
        }
    }
}
