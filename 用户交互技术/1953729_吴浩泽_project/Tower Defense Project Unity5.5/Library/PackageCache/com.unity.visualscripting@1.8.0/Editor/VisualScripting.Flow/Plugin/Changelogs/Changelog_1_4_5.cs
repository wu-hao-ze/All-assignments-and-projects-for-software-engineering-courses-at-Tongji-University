using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_5 : PluginChangelog
    {
        public Changelog_1_4_5(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.5";

        public override DateTime date => new DateTime(2019, 07, 15);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Unity crash due to Mono runtime vtable issue with type unification";
            }
        }
    }
}
