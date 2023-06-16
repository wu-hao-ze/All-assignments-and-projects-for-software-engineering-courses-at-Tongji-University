using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class InspectorAdaptiveWidthAttribute : Attribute
    {
        public InspectorAdaptiveWidthAttribute(float width)
        {
            this.width = width;
        }

        public float width { get; private set; }
    }
}
