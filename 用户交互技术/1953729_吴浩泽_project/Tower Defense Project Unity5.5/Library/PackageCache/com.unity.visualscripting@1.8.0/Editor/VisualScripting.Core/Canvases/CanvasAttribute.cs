using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CanvasAttribute : Attribute, IDecoratorAttribute
    {
        public CanvasAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; private set; }
    }
}
