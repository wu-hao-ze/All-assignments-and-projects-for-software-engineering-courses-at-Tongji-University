using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class GraphContextExtensionAttribute : Attribute, IDecoratorAttribute
    {
        public GraphContextExtensionAttribute(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);
            Ensure.That(nameof(type)).IsOfType(type, typeof(IGraphContext));

            this.type = type;
        }

        public Type type { get; }
    }
}
