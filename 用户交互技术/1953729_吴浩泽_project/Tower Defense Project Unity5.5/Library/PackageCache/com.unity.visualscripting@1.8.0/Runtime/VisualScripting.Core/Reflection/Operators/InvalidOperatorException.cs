using System;

namespace Unity.VisualScripting
{
    public sealed class InvalidOperatorException : OperatorException
    {
        public InvalidOperatorException(string symbol, Type type) : base($"Operator '{symbol}' cannot be applied to operand of type '{type?.ToString() ?? "null"}'.") { }
        public InvalidOperatorException(string symbol, Type leftType, Type rightType) : base($"Operator '{symbol}' cannot be applied to operands of type '{leftType?.ToString() ?? "null"}' and '{rightType?.ToString() ?? "null"}'.") { }
    }
}
