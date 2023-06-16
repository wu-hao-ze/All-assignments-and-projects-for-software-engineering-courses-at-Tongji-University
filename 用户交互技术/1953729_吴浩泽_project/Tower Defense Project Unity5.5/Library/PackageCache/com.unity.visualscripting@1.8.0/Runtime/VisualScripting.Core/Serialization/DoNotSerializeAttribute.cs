using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DoNotSerializeAttribute : Attribute
    {
        public DoNotSerializeAttribute() { }
    }
}
