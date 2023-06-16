using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class EditorPrefMetadata : PluginConfigurationItemMetadata
    {
        public EditorPrefMetadata(PluginConfiguration configuration, MemberInfo member, Metadata parent) : base(configuration, member, parent) { }

        public string namespacedKey => GetNamespacedKey(configuration.plugin.id, key);

        public override bool exists => EditorPrefs.HasKey(namespacedKey);

        public override void Load()
        {
            try
            {
                value = new SerializationData(EditorPrefs.GetString(namespacedKey)).Deserialize();
            }
            catch (Exception)
            {
                Debug.LogWarning($"Failed to deserialize editor pref '{configuration.plugin.id}.{key}', reverting to default.\nYou can set it again by going to Edit -> Preferences -> Visual Scripting.");
                value = defaultValue;
                Save();
            }

            if (!definedType.IsAssignableFrom(valueType))
            {
                Debug.LogWarning($"Failed to deserialize editor pref '{configuration.plugin.id}.{key}' as '{definedType.CSharpName()}', reverting to default.\nYou can set it again by going to Edit -> Preferences -> Visual Scripting.");
                value = defaultValue;
                Save();
            }
        }

        internal override void SaveImmediately(bool immediately = true)
        {
            Save();
        }

        public override void Save()
        {
            EditorPrefs.SetString(namespacedKey, value.Serialize().json);
        }

        public static string GetNamespacedKey(string pluginId, string key)
        {
            return $"{pluginId}.{key}";
        }
    }
}
