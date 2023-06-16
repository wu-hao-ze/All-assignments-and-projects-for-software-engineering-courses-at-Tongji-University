using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_1_2 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_1_2(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.1.2";
        public override DateTime date => new DateTime(2017, 10, 16);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Edge pan for resize, group, select and connections";
                yield return "[Added] Keyboard shortcut for contextual menu ([Ctrl / Cmd] + E)";
                yield return "[Optimized] Element collection memory allocation";
                yield return "[Optimized] Instantiation performance and memory allocation";
                yield return "[Fixed] Selection lasso not closing out of graph window​";
            }
        }
    }
}
