using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    internal class MigrationUtility_1_5_1_to_1_5_2
    {
        internal static DictionaryAsset GetLegacyProjectSettingsAsset(string pluginId)
        {
            try
            {
                var settingsAssetPath = Path.Combine("Assets", "Bolt.Generated", pluginId, "ProjectSettings.asset");
                var asset = AssetDatabase.LoadAssetAtPath<DictionaryAsset>(settingsAssetPath);
                return asset;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static SemanticVersion TryManualParseSavedVersion(string pluginId)
        {
            try
            {
                var vsCoreProjectSettings = GetLegacyProjectSettingsAsset("VisualScripting.Core");
                if (vsCoreProjectSettings == null)
                    return new SemanticVersion();

                return (SemanticVersion)vsCoreProjectSettings["savedVersion"];
            }
            catch (Exception)
            {
                return new SemanticVersion();
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Migration_1_5_1_to_1_5_2 : PluginMigration
    {
        internal Migration_1_5_1_to_1_5_2(Plugin plugin) : base(plugin)
        {
            order = 1;
        }

        public override SemanticVersion @from => "1.5.1";
        public override SemanticVersion to => "1.5.2";

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

            try
            {
                MigrateVariablesAssets();
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                Debug.LogWarning("There was a problem migrating your Visual Scripting application or saved variables. You might want to restore your backup");
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                Debug.LogError(e);
#endif
            }
        }

        private static void MigrateProjectSettings()
        {
            BoltCore.Configuration.LoadOrCreateProjectSettingsAsset();

            var legacyProjectSettingsAsset = MigrationUtility_1_5_1_to_1_5_2.GetLegacyProjectSettingsAsset("VisualScripting.Core");
            if (legacyProjectSettingsAsset != null)
            {
                BoltCore.Configuration.projectSettingsAsset.Merge(legacyProjectSettingsAsset);
            }

            BoltCore.Configuration.SaveProjectSettingsAsset(true);
            BoltCore.Configuration.ResetProjectSettingsMetadata();
        }

        private static void MigrateVariablesAssets()
        {
            // We have application and saved variables to migrate
            var variableAssetNames = new string[] { "ApplicationVariables", "SavedVariables" };

            foreach (var fileName in variableAssetNames)
            {
                var legacyAssetPath = Path.Combine(Paths.assets, "Bolt.Generated", "VisualScripting.Core", "Variables", "Resources", fileName + ".asset");
                var newAssetPath = Path.Combine(Paths.assets, "Unity.VisualScripting.Generated", "VisualScripting.Core", "Variables", "Resources", fileName + ".asset");
                var directory = Path.GetDirectoryName(newAssetPath);

                if (File.Exists(legacyAssetPath) && !File.Exists(newAssetPath))
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.Copy(legacyAssetPath, newAssetPath);
                    File.Move(legacyAssetPath + ".meta", newAssetPath + ".meta");
                }
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Migration_1_5_1_to_1_5_2_Post : PluginMigration
    {
        internal Migration_1_5_1_to_1_5_2_Post(Plugin plugin) : base(plugin)
        {
            order = 3;
        }

        public override SemanticVersion @from => "1.5.1";
        public override SemanticVersion to => "1.5.2";

        public override void Run()
        {
            CleanupLegacyFiles();

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private static void CleanupLegacyFiles()
        {
            var legacyGeneratedFolderAssetPath = Path.Combine("Assets", "Bolt.Generated");

            AssetDatabase.DeleteAsset(legacyGeneratedFolderAssetPath);
        }
    }

    [Plugin(BoltCore.ID)]
    internal class DeprecatedSavedVersionLoader_1_5_1 : PluginDeprecatedSavedVersionLoader
    {
        internal DeprecatedSavedVersionLoader_1_5_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.5.1";

        public override bool Run(out SemanticVersion savedVersion)
        {
            var manuallyParsedVersion = MigrationUtility_1_5_1_to_1_5_2.TryManualParseSavedVersion("VisualScripting.Core");
            savedVersion = manuallyParsedVersion;

            return savedVersion != "0.0.0";
        }
    }
}
