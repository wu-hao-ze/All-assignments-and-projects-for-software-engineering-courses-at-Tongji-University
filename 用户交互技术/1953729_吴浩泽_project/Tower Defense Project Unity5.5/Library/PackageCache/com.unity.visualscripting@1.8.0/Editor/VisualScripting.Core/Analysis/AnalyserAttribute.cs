using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class AnalyserAttribute : Attribute, IDecoratorAttribute
    {
        public AnalyserAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; private set; }
    }
}
