using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class InspectorAttribute : Attribute, IDecoratorAttribute
    {
        public InspectorAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; private set; }
    }
}
