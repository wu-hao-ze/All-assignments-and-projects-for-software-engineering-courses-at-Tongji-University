using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class GraphContextAttribute : Attribute, IDecoratorAttribute
    {
        public GraphContextAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; private set; }
    }
}
