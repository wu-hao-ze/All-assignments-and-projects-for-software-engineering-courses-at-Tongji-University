using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Changelog_1_0_2 : PluginChangelog
    {
        public Changelog_1_0_2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.0.2";
        public override DateTime date => new DateTime(2017, 09, 08);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Order-of-operations issues with transitions and updates";
            }
        }
    }
}
