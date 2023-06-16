using System;

namespace Unity.VisualScripting
{
    static class TypeExtensions
    {
        public static SerializableType GenerateTypeHandle(this Type type)
        {
            return type == typeof(Unknown)
                ? new SerializableType(Unknown.Identification)
                : TypeSerializer.GenerateTypeHandle(type);
        }
    }
}
