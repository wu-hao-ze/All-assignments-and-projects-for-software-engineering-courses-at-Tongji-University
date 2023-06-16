using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_5 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_5(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.5";

        public override DateTime date => new DateTime(2019, 07, 15);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Warning message when converting embed graphs that contain scene references to macros";
            }
        }
    }
}
