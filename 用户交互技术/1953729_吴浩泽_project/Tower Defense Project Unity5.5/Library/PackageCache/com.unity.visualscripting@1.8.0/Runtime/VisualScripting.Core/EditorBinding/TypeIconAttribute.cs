using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class TypeIconAttribute : Attribute
    {
        public TypeIconAttribute(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            this.type = type;
        }

        public Type type { get; }
    }
}
