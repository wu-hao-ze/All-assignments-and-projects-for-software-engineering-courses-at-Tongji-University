using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_1_0 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_1_0(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.1.0";
        public override DateTime date => new DateTime(2017, 10, 03);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Shortcut to navigate to parent graph";
                yield return "[Added] Zoom to selection keyboard shortcut";
                yield return "[Added] Zoom graph to cursor position";
                yield return "[Added] Automatic edge pan";
                yield return "[Added] Shift to lock drag axis";
                yield return "[Added] Resize group from top edge";
                yield return "[Added] Recenter elements on paste";
                yield return "[Fixed] Dragging when duplicating groups";
            }
        }
    }
}
