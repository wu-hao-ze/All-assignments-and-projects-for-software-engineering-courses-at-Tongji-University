using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_0_3 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_0_3(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.0.3";
        public override DateTime date => new DateTime(2017, 08, 09);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Graph title and summary in machine inspector";
                yield return "[Fixed] Breadcrumb nesting issue when editing graphs on the same object";
            }
        }
    }
}
