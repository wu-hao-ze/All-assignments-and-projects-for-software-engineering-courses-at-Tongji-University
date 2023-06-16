using System;

namespace Unity.VisualScripting
{
    static class SerializableTypeExtensions
    {
        public static Type Resolve(this SerializableType self)
        {
            return TypeSerializer.ResolveType(self);
        }
    }
}
