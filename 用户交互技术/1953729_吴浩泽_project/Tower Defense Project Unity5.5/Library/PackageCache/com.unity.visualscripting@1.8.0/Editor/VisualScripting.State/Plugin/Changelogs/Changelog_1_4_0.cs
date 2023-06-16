using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Changelog_1_4_0f6 : PluginChangelog
    {
        public Changelog_1_4_0f6(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f6";
        public override DateTime date => new DateTime(2018, 09, 06);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] On Enter State and On Exit State events not firing in super units";
            }
        }
    }

    [Plugin(BoltState.ID)]
    internal class Changelog_1_4_0f10 : PluginChangelog
    {
        public Changelog_1_4_0f10(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f10";
        public override DateTime date => new DateTime(2018, 10, 29);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Inactive states starting to listen after undo";
            }
        }
    }
}
