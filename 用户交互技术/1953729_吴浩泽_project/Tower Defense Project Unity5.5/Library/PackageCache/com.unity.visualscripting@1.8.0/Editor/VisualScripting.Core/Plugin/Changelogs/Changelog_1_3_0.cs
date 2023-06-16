using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Changelog_1_3_0 : PluginChangelog
    {
        public Changelog_1_3_0(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.3.0";
        public override DateTime date => new DateTime(2018, 04, 06);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Refactored] Moved Variables and Events to core assembly";
                yield return "[Fixed] Error when merging a saved variable to a null initial variable";
            }
        }
    }
}
