using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_2 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.2";

        public override DateTime date => new DateTime(2019, 04, 03);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Wrong window focusing when clicking Edit Graph and multiple graph windows are open";
            }
        }
    }
}
