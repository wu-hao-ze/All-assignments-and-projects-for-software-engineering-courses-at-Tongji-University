using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_1_1 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_1_1(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.1.1";
        public override DateTime date => new DateTime(2017, 10, 10);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Changed] Limited double-click focus to when zoomed out";
                yield return "[Fixed] Delete events in text fields deleting graph elements";
                yield return "[Fixed] Recursive element description not updating";
            }
        }
    }
}
