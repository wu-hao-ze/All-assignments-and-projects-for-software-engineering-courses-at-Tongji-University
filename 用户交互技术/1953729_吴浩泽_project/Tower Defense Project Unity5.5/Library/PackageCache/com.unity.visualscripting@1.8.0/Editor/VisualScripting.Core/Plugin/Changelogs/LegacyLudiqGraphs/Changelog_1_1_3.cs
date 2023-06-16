using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_1_3 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_1_3(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.1.3";
        public override DateTime date => new DateTime(2017, 10, 30);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Lock current graph button and editor preference";
                yield return "[Added] Edit contextual menu actions for individual nodes";
                yield return "[Added] Edge pan support for resize";
                yield return "[Fixed] Undo failure in nested embed graphs";
                yield return "[Fixed] Issues with mouse drag and lasso";
                yield return "[Fixed] Issues graph selection change detection";
            }
        }
    }
}
