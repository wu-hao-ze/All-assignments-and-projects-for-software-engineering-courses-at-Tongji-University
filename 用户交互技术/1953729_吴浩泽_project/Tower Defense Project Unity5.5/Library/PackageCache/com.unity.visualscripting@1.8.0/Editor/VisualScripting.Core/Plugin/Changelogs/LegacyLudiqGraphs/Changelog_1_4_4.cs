using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_4 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_4(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.4";

        public override DateTime date => new DateTime(2019, 06, 11);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Prewarming routine not getting called on machines";
            }
        }
    }
}
