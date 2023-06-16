using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public sealed class IncludeInSettingsAttribute : Attribute
    {
        public IncludeInSettingsAttribute(bool include)
        {
            this.include = include;
        }

        public bool include { get; private set; }
    }
}
