using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PortKeyAttribute : Attribute
    {
        public PortKeyAttribute(string key)
        {
            Ensure.That(nameof(key)).IsNotNull(key);

            this.key = key;
        }

        public string key { get; }
    }
}
