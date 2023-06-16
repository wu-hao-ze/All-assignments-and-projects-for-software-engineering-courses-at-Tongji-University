using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginModuleAttribute : Attribute
    {
        public bool required { get; set; } = true;
    }
}
