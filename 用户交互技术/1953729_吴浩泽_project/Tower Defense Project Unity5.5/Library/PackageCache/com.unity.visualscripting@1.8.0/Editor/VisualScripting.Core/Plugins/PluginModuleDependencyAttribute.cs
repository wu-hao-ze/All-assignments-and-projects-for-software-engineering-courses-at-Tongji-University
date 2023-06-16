using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PluginModuleDependencyAttribute : Attribute
    {
        public PluginModuleDependencyAttribute(Type moduleType)
        {
            if (moduleType == null)
            {
                throw new ArgumentNullException(nameof(moduleType));
            }

            this.moduleType = moduleType;
        }

        public Type moduleType { get; private set; }
    }
}
