using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_0_1 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_0_1(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.0.1";
        public override DateTime date => new DateTime(2017, 07, 29);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Legibility and resolution issues on pro skin";
                yield return "[Changed] Ctrl bindings to Cmd on OSX";
            }
        }
    }
}
