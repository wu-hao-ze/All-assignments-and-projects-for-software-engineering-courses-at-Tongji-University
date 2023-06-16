using System;

namespace Unity.VisualScripting
{
    // Allows us to migrate old serialized namespaces to new ones
    // Ex usage: [assembly: RenamedAssembly("Bolt.Core", "Unity.VisualScripting.Core")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class RenamedAssemblyAttribute : Attribute
    {
        public RenamedAssemblyAttribute(string previousName, string newName)
        {
            this.previousName = previousName;
            this.newName = newName;
        }

        public string previousName { get; }

        public string newName { get; }
    }
}
