using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class DescriptorAttribute : Attribute, IDecoratorAttribute
    {
        public DescriptorAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; private set; }
    }
}
