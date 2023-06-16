using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class InspectorWideAttribute : Attribute
    {
        public InspectorWideAttribute() { }

        public InspectorWideAttribute(bool toEdge)
        {
            this.toEdge = toEdge;
        }

        public bool toEdge { get; private set; }
    }
}
