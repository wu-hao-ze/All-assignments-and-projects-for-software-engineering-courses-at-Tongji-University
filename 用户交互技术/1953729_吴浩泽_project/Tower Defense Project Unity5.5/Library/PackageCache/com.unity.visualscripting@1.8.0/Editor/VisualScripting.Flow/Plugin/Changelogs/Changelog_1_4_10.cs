using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_10 : PluginChangelog
    {
        public Changelog_1_4_10(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.10";

        public override DateTime date => new DateTime(2019, 12, 13);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Changed] Behaviour of Wait Until and Wait While unit to check the condition on their entry flow instead of creating new flows at every frame";
                yield return "[Fixed] Threading exception when comparing destroyed self object on background thread in unit option tree";
            }
        }
    }
}
