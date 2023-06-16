using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_0_2 : PluginChangelog
    {
        public Changelog_1_0_2(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.0.2";
        public override DateTime date => new DateTime(2017, 08, 08);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Adding units from fuzzy finder not registering an undo";
                yield return "[Fixed] Serialization issue with unit definitions";
                yield return "[Fixed] Casting error in missing component prediction";
                yield return "[Fixed] Missing default values after nested macro deserialization";
                yield return "[Fixed] Events in super units not listening";
            }
        }
    }
}
