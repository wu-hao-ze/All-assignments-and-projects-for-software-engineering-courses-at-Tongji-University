using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class FuzzyOptionAttribute : Attribute, IDecoratorAttribute
    {
        public FuzzyOptionAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; }
    }
}
