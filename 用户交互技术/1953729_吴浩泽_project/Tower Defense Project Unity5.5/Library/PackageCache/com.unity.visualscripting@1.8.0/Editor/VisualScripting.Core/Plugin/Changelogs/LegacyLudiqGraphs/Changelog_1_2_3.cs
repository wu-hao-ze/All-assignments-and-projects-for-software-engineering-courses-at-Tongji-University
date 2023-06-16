using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_2_3 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_2_3(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.2.3";
        public override DateTime date => new DateTime(2018, 01, 25);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Refactored] Canvas code to use control IDs";
                yield return "[Fixed] Quick window dragging causing lasso";
                yield return "[Fixed] Mouse issues in canvas";
            }
        }
    }
}
