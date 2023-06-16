using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class InspectorActionDirectionAttribute : Attribute
    {
        public InspectorActionDirectionAttribute(ActionDirection direction)
        {
            this.direction = direction;
        }

        public ActionDirection direction { get; private set; }
    }
}
