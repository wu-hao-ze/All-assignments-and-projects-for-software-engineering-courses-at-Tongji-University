using System.Reflection;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class ProjectSettingMetadata : PluginConfigurationItemMetadata
    {
        public ProjectSettingMetadata(PluginConfiguration configuration, MemberInfo member, Metadata parent) : base(configuration, member, parent) { }

        public override bool exists => storage != null && storage.ContainsKey(key);

        private DictionaryAsset storage => configuration.projectSettingsAsset;

        public override void Load()
        {
            if (!definedType.IsAssignableFrom(valueType))
            {
                Debug.LogWarning($"Failed to cast project setting '{configuration.plugin.id}.{key}' as '{definedType.CSharpFullName()}', reverting to default.");
                value = defaultValue;
            }

            value = storage[key];
        }
        internal override void SaveImmediately(bool immediately = true)
        {
            if (storage == null)
                configuration.LoadOrCreateProjectSettingsAsset();
            if (storage.ContainsKey(key))
            {
                storage[key] = value;
            }
            else
            {
                storage.Add(key, value);
            }

            configuration.SaveProjectSettingsAsset(immediately);
        }

        public override void Save()
        {
            SaveImmediately(false);
        }
    }
}
