using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_4_9 : PluginChangelog
    {
        public LudiqCoreChangelog_1_4_9(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.9";

        public override DateTime date => new DateTime(2019, 11, 04);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Graphs failing to load when they included a newly created macro (the legendary 'undo bug')";
            }
        }
    }
}
