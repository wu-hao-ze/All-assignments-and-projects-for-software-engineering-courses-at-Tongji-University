using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    internal class MigrationUtility_1_6_to_1_7
    {
        public static DictionaryAsset GetLegacyProjectSettingsAsset(string pluginId)
        {
            try
            {
                var settingsFullPath = Path.Combine(Paths.assets, "Unity.VisualScripting.Generated", pluginId, "ProjectSettings.asset");
                var settingsAssetPath = Path.Combine("Assets", PathUtility.FromAssets(settingsFullPath));
                var asset = AssetDatabase.LoadAssetAtPath<DictionaryAsset>(settingsAssetPath);
                return asset;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Migration_1_6_to_1_7 : PluginMigration
    {
        internal Migration_1_6_to_1_7(Plugin plugin) : base(plugin)
        {
            order = 1;
        }

        public override SemanticVersion @from => "1.6.1000";
        public override SemanticVersion to => "1.7.0-pre.0";

        public override void Run()
        {
            try
            {
                MigrateProjectSettings();
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                Debug.LogWarning("There was a problem migrating your Visual Scripting project settings. Be sure to check them in Edit -> Project Settings -> Visual Scripting");
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                Debug.LogError(e);
#endif
            }

            AddFeatureSetAssembliesToAssemblyList();
        }

        private static void MigrateProjectSettings()
        {
            var legacyProjectSettingPluginIds = new string[]
            {"VisualScripting.Core", "VisualScripting.Flow", "VisualScripting.State"};

            BoltCore.Configuration.LoadOrCreateProjectSettingsAsset();

            foreach (var pluginId in legacyProjectSettingPluginIds)
            {
                var legacyProjectSettingsAsset =
                    MigrationUtility_1_6_to_1_7.GetLegacyProjectSettingsAsset(pluginId);
                if (legacyProjectSettingsAsset != null)
                {
                    BoltCore.Configuration.projectSettingsAsset.Merge(legacyProjectSettingsAsset);
                }
            }

            BoltCore.Configuration.SaveProjectSettingsAsset(true);
            BoltCore.Configuration.ResetProjectSettingsMetadata();
        }

        private void AddFeatureSetAssembliesToAssemblyList()
        {
            var newAssemblyOptions = new string[]
            {
                // Timeline
                "Unity.Timeline",
                "Unity.Timeline.Editor",

                // Cinemachine
                "Cinemachine",
                "com.unity.cinemachine.Editor",

                // Input System
                "Unity.InputSystem",
            };

            foreach (var assemblyOption in newAssemblyOptions)
            {
                if (!BoltCore.Configuration.assemblyOptions.Contains(assemblyOption))
                {
                    BoltCore.Configuration.assemblyOptions.Add(assemblyOption);
                }
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class DeprecatedSavedVersionLoader_1_6_1 : PluginDeprecatedSavedVersionLoader
    {
        public DeprecatedSavedVersionLoader_1_6_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.6.1";

        public override bool Run(out SemanticVersion savedVersion)
        {
            savedVersion = new SemanticVersion();
            try
            {
                var legacyProjectSettingsAsset = MigrationUtility_1_6_to_1_7.GetLegacyProjectSettingsAsset("VisualScripting.Core");
                if (legacyProjectSettingsAsset == null)
                    return false;

                savedVersion = (SemanticVersion)legacyProjectSettingsAsset["savedVersion"];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
