using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PluginDependencyAttribute : Attribute
    {
        public PluginDependencyAttribute(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            this.id = id;
        }

        public string id { get; private set; }
    }
}
