using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Changelog_1_2_3 : PluginChangelog
    {
        public Changelog_1_2_3(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.2.3";
        public override DateTime date => new DateTime(2018, 01, 25);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Trigger enter / exit state events in transitions";
                yield return "[Fixed] Fixed Update and Late Update not firing in super states";
            }
        }
    }
}
