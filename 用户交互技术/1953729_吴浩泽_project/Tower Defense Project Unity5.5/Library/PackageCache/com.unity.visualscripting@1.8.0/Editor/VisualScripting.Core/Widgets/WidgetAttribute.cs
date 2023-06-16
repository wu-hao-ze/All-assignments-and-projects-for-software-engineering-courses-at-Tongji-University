using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class WidgetAttribute : Attribute, IDecoratorAttribute
    {
        public WidgetAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; private set; }
    }
}
