using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_0_3 : PluginChangelog
    {
        public Changelog_1_0_3(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.0.3";
        public override DateTime date => new DateTime(2017, 09, 08);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Unit database system to speed up loading times";
                yield return "[Added] Unit options wizard";
                yield return "[Added] GUI pointer events";
                yield return "[Fixed] Events not firing recursively";
                yield return "[Fixed] Hierarchy issues with the unit option tree";
                yield return "[Fixed] Error with super unit control output definition";
                yield return "[Fixed] Macro instance sharing issues";
                yield return "[Fixed] Recursive graph issues";
            }
        }
    }
}
