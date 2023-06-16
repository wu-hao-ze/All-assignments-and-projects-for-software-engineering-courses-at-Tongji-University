using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class SerializedPropertyProviderAttribute : Attribute, IDecoratorAttribute
    {
        public SerializedPropertyProviderAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; private set; }
    }
}
