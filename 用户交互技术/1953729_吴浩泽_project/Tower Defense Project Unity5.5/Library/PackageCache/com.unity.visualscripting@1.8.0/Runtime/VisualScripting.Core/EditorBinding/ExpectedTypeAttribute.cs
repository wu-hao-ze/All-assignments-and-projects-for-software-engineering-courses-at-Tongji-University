using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ExpectedTypeAttribute : Attribute
    {
        public ExpectedTypeAttribute(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            this.type = type;
        }

        public Type type { get; }
    }
}
