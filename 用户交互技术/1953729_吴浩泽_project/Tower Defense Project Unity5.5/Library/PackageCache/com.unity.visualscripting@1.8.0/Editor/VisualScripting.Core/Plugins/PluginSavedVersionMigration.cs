using System;

namespace Unity.VisualScripting
{
    // This is for whenever the PluginConfiguration's savedVersion has been moved or had its format changed.
    // Is necessary because in order for us to run Plugin Migrations (See PluginMigration class), we need to know the existing
    // version of Bolt in the project. If the place where we store that version number has changed, or the format we store it in has changed,
    // we can potentially lose the existing version and fail to run the correct migrations.
    // PluginDeprecatedSavedVersionLoaders only restore the existing Bolt savedVersion if it exists in an earlier format / location so that we
    // can run the appropriate migrations.
    public abstract class PluginDeprecatedSavedVersionLoader : IPluginLinked, IComparable<PluginDeprecatedSavedVersionLoader>
    {
        protected PluginDeprecatedSavedVersionLoader(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public Plugin plugin { get; }

        public abstract SemanticVersion from { get; }

        public abstract bool Run(out SemanticVersion savedVersion);

        public int CompareTo(PluginDeprecatedSavedVersionLoader other)
        {
            return from.CompareTo(other.from);
        }
    }
}
