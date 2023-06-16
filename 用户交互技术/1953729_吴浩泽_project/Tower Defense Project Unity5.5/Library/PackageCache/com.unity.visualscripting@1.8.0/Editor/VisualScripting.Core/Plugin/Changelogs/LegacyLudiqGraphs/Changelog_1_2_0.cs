using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_2_0 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_2_0(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.2.0";
        public override DateTime date => new DateTime(2017, 11, 16);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Drag & drop system and preview label for all operations";
                yield return "[Added] Comments for graph groups";
                yield return "[Added] Custom colors for graph groups";
                yield return "[Added] Inspector and icon for graph groups";
                yield return "[Added] Title and summary editing from parent inspector";
                yield return "[Fixed] Context menu and fuzzy finder not opening when zoomed out";
                yield return "[Fixed] Error after using Shift+Space to maximize graph window";
                yield return "[Fixed] Lasso starting when dragging in the graph window header";
                yield return "[Fixed] Various mouse drag issues";
                yield return "[Fixed] Selection change event triggering unduly";
                yield return "[Changed] Lowered edge pan offset";
                yield return "[Changed] Pasting from context menu now pastes to mouse position";
            }
        }
    }
}
