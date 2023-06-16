using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class FuzzyOptionTreeExtensionAttribute : Attribute, IDecoratorAttribute
    {
        public FuzzyOptionTreeExtensionAttribute(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);
            Ensure.That(nameof(type)).IsOfType(type, typeof(IFuzzyOptionTree));

            this.type = type;
        }

        public Type type { get; }
    }
}
