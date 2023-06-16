using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class EditorAttribute : Attribute, IDecoratorAttribute
    {
        public EditorAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; private set; }
    }
}
