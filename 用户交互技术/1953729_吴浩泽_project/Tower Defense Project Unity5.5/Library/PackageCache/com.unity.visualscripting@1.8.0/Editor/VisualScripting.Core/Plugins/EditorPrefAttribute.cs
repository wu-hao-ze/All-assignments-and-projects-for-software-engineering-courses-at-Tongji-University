namespace Unity.VisualScripting
{
    public sealed class EditorPrefAttribute : PluginConfigurationItemAttribute
    {
        public EditorPrefAttribute() : base() { }

        public EditorPrefAttribute(string key) : base(key) { }
    }
}
