using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Changelog_1_0_1 : PluginChangelog
    {
        public Changelog_1_0_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.0.1";
        public override DateTime date => new DateTime(2017, 08, 01);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] State header icon size on retina displays";
                yield return "[Fixed] Pasting into state transition";
                yield return "[Fixed] Transition events not being triggered from state entry";
            }
        }
    }
}
