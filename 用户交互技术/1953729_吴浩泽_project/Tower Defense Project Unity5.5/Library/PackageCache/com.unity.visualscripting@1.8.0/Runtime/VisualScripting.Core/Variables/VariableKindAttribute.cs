using System;

namespace Unity.VisualScripting
{
    [Obsolete("Set VariableKind via VariableDeclarations.Kind")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class VariableKindAttribute : Attribute
    {
        public VariableKindAttribute(VariableKind kind)
        {
            this.kind = kind;
        }

        public VariableKind kind { get; }
    }
}
