using System;

namespace Unity.VisualScripting
{
    // Allows us to migrate old serialized namespaces to new ones
    // Ex usage: [assembly: RenamedNamespace("Bolt", "Unity.VisualScripting")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class RenamedNamespaceAttribute : Attribute
    {
        public RenamedNamespaceAttribute(string previousName, string newName)
        {
            this.previousName = previousName;
            this.newName = newName;
        }

        public string previousName { get; }

        public string newName { get; }
    }
}
