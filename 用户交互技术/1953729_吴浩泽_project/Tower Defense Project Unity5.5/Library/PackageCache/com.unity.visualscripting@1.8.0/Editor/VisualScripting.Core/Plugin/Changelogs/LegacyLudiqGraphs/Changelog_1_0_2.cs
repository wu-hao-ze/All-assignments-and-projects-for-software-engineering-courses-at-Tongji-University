using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_0_2 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_0_2(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.0.2";
        public override DateTime date => new DateTime(2017, 08, 01);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Error when entering nested graphs after a paste operation";
                yield return "[Fixed] Error when converting from macro to embed graph";
            }
        }
    }
}
