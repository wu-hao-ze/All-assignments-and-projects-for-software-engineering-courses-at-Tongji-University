using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Migration_Asset_to_Package : PluginMigration
    {
        public Migration_Asset_to_Package(Plugin plugin) : base(plugin)
        {
            order = 2;
        }

        public override SemanticVersion @from => "1.4.1000";
        public override SemanticVersion to => "1.5.0-pre.0";

        public override void Run()
        {
            plugin.configuration.Initialize();

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
                MigrationUtility_Asset_to_Package.MigrateEditorPreferences(this.plugin);
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                Debug.LogWarning("There was a problem migrating your Visual Scripting editor preferences. Be sure to check them in Edit -> Preferences -> Visual Scripting");
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                Debug.LogError(e);
#endif
            }
        }

        private static void MigrateProjectSettings()
        {
            // Bolt.State -> VisualScripting.State
            BoltState.Configuration.LoadOrCreateProjectSettingsAsset();

            var legacyProjectSettingsAsset = MigrationUtility_Asset_to_Package.GetLegacyProjectSettingsAsset("Bolt.State");
            if (legacyProjectSettingsAsset != null)
            {
                BoltState.Configuration.projectSettingsAsset.Merge(legacyProjectSettingsAsset);
            }

            BoltState.Configuration.SaveProjectSettingsAsset(true);
            BoltState.Configuration.ResetProjectSettingsMetadata();
        }
    }

    [Plugin(BoltState.ID)]
    internal class DeprecatedSavedVersionLoader_Bolt_AssetStore : PluginDeprecatedSavedVersionLoader
    {
        public DeprecatedSavedVersionLoader_Bolt_AssetStore(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.4.13";

        public override bool Run(out SemanticVersion savedVersion)
        {
            var manuallyParsedVersion = MigrationUtility_Asset_to_Package.TryManualParseSavedVersion("Bolt.State");
            savedVersion = manuallyParsedVersion;

            return savedVersion != "0.0.0";
        }
    }
}
