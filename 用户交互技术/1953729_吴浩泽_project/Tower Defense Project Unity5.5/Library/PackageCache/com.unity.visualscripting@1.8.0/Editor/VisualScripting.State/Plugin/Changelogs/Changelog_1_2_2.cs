using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Changelog_1_2_2 : PluginChangelog
    {
        public Changelog_1_2_2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.2.2";
        public override DateTime date => new DateTime(2017, 12, 04);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Any State";
                yield return "[Added] Droplet animations for transitions";
            }
        }
    }
}
