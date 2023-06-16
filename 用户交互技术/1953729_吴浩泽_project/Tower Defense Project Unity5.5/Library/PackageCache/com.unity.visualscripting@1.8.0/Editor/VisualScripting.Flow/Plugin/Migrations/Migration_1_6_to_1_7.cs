using System;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Migration_1_6_to_1_7 : PluginMigration
    {
        internal Migration_1_6_to_1_7(Plugin plugin) : base(plugin)
        {
            order = 2;
        }

        public override SemanticVersion @from => "1.6.1000";
        public override SemanticVersion to => "1.7.0-pre.0";

        public override void Run()
        {
            // Need to reset our project settings metadata list to point to the new project settings asset and
            // underlying dictionary. That way when it gets saved we don't overwrite the files data
            BoltFlow.Configuration.LoadProjectSettings();

            UnitBase.Rebuild();
        }
    }

    [Plugin(BoltFlow.ID)]
    internal class DeprecatedSavedVersionLoader_1_6_1 : PluginDeprecatedSavedVersionLoader
    {
        public DeprecatedSavedVersionLoader_1_6_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.6.1";

        public override bool Run(out SemanticVersion savedVersion)
        {
            savedVersion = new SemanticVersion();
            try
            {
                var legacyProjectSettingsAsset = MigrationUtility_1_6_to_1_7.GetLegacyProjectSettingsAsset("VisualScripting.Flow");
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
