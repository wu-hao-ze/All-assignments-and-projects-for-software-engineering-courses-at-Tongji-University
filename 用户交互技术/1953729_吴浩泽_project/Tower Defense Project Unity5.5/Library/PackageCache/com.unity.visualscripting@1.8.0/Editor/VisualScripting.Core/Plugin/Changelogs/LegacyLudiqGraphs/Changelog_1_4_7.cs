using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_7 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_7(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.7";

        public override DateTime date => new DateTime(2019, 09, 26);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Clipping errors when opening graph window as a tab in Peek";
            }
        }
    }
}
