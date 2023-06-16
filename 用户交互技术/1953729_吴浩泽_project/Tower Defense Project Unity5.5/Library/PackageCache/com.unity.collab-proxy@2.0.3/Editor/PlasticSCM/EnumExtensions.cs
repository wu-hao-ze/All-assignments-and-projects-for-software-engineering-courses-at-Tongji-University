using System;

namespace Unity.PlasticSCM.Editor
{
    internal static class EnumExtensions
    {
        internal static bool HasFlag(this Enum variable, Enum value)
        {
            if (variable.GetType() != value.GetType())
                throw new ArgumentException(
                    "The checked flag is not from the same type as the checked variable.");

            Convert.ToUInt64(value);
            ulong num = Convert.ToUInt64(value);
            ulong num2 = Convert.ToUInt64(variable);

            return (num2 & num) == num;
        }
    }
}
