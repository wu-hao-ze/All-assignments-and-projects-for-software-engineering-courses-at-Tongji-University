using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class AotStubWriterAttribute : Attribute, IDecoratorAttribute
    {
        public AotStubWriterAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; private set; }
    }
}
