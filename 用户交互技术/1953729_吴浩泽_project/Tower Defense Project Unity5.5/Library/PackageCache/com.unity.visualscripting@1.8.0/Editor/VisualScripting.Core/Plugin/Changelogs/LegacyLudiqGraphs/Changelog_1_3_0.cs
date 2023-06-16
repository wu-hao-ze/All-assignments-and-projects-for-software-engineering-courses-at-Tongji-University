using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_3_0 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_3_0(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.3.0";
        public override DateTime date => new DateTime(2018, 04, 06);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Refactored] Graph element collections";
                yield return "[Refactored] Graph context and breadcrumbs";
                yield return "[Added] Dynamic element dependency resolution";
                yield return "[Fixed] Undo issues";
                yield return "[Fixed] Graph context validation issues";
                yield return "[Added] Dim disabling when mouse over or selected";
                yield return "[Added] Dim fade animation";
                yield return "[Fixed] Snap to grid when resizing min axes";
                yield return "[Fixed] Graph inspector height calculation";
            }
        }
    }
}
