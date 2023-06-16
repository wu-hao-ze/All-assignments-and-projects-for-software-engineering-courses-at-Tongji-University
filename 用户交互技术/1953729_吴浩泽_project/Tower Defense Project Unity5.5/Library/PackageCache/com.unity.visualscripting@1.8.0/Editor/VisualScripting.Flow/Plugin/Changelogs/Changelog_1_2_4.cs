using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_2_4 : PluginChangelog
    {
        public Changelog_1_2_4(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.2.4";
        public override DateTime date => new DateTime(2018, 02, 26);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Missing scene variable options";
                yield return "[Fixed] Set scene variable causing exception";
                yield return "[Changed] Coroutine runner to parent game object";
            }
        }
    }
}
