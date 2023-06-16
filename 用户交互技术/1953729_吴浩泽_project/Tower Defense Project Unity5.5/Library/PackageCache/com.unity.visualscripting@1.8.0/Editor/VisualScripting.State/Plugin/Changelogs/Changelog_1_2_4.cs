using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Changelog_1_2_4 : PluginChangelog
    {
        public Changelog_1_2_4(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.2.4";
        public override DateTime date => new DateTime(2018, 02, 26);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Manual events not triggering in state units";
            }
        }
    }
}
