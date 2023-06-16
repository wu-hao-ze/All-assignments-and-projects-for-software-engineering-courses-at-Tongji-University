namespace Unity.VisualScripting
{
    public class ProjectSettingAttribute : PluginConfigurationItemAttribute
    {
        public ProjectSettingAttribute() : base() { }

        public ProjectSettingAttribute(string key) : base(key) { }
    }
}
