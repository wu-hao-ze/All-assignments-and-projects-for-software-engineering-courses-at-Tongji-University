using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_6 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_6(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.6";

        public override DateTime date => new DateTime(2019, 08, 20);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Crash when instantiating recursive graphs";
            }
        }
    }
}
