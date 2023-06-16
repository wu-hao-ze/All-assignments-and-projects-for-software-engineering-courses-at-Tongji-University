using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class PluginConfigurationItemAttribute : Attribute
    {
        protected PluginConfigurationItemAttribute() { }

        protected PluginConfigurationItemAttribute(string key)
        {
            this.key = key;
        }

        public string key { get; }

        public bool visible { get; set; } = true;
        public bool enabled { get; set; } = true;
        public bool resettable { get; set; } = true;

        public string visibleCondition { get; set; } = null;
        public string enabledCondition { get; set; } = null;
        public string resettableCondition { get; set; } = null;
    }
}
