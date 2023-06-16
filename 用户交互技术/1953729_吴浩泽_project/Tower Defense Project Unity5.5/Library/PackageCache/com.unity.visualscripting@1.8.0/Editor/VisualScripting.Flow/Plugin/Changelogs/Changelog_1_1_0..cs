using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_1_0 : PluginChangelog
    {
        public Changelog_1_1_0(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.1.0";
        public override DateTime date => new DateTime(2017, 10, 03);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Active connections animation";
                yield return "[Added] Active node colors fade out";
                yield return "[Added] Smart output contextual options";
                yield return "[Added] Prominent unit settings labels";
                yield return "[Added] Units resize based on content";
                yield return "[Fixed] Scene variables disappearing";
            }
        }
    }
}
