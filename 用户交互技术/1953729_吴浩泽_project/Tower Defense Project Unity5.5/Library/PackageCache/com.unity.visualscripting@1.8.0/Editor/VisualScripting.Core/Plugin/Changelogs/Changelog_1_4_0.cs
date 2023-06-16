using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Changelog_1_4_0 : PluginChangelog
    {
        public Changelog_1_4_0(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0";
        public override DateTime date => new DateTime(2018, 05, 16);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Definition and Instance tabs for graph variables";
                yield return "[Added] Prefab and Instance tabs for object variables";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Changelog_1_4_0f2 : PluginChangelog
    {
        public Changelog_1_4_0f2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f2";
        public override DateTime date => new DateTime(2018, 07, 13);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Events still firing after being unregistered during same trigger";
                yield return "[Fixed] OnEnable being called twice";
                yield return "[Fixed] Search prewarm and loading delay";
                yield return "[Fixed] Reorderable list control textures on linear space";
                yield return "[Fixed] API thread error during time / frame fetching";
                yield return "[Improved] Unit options loading error recovery";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Changelog_1_4_0f3 : PluginChangelog
    {
        public Changelog_1_4_0f3(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f3";
        public override DateTime date => new DateTime(2018, 07, 31);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] OnParticleCollision event not firing";
                yield return "[Fixed] OnDropdownValueChanged event error";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Changelog_1_4_0f6 : PluginChangelog
    {
        public Changelog_1_4_0f6(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f6";
        public override DateTime date => new DateTime(2018, 09, 06);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Variables window not updating on mode change when no graph was selected";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Changelog_1_4_0f7 : PluginChangelog
    {
        public Changelog_1_4_0f7(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f7";
        public override DateTime date => new DateTime(2018, 09, 25);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Manual event triggering causing pointer data error in edit mode";
            }
        }
    }
}
