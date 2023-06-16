using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_2_4 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_2_4(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.2.4";
        public override DateTime date => new DateTime(2018, 02, 26);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Optimized] Graph editor";
                yield return "[Obsoleted] Background embed graphs";
                yield return "[Added] New button for macro field";
                yield return "[Changed] Default machine source to macro instead of embed";
                yield return "[Fixed] Event.Use warning when deleting graph elements";
                yield return "[Added] Self prediction for macro graphs on machines";
                yield return "[Fixed] Node position undo";
                yield return "[Fixed] Vertical resize";
            }
        }
    }
}
