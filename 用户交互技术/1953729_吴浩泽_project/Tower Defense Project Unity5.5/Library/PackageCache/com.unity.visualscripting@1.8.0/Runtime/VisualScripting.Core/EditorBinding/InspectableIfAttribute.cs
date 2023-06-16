using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public sealed class InspectableIfAttribute : Attribute, IInspectableAttribute
    {
        public InspectableIfAttribute(string conditionMember)
        {
            this.conditionMember = conditionMember;
        }

        public int order { get; set; }

        public string conditionMember { get; }
    }
}
