using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_6 : PluginChangelog
    {
        public Changelog_1_4_6(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.6";

        public override DateTime date => new DateTime(2019, 08, 20);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Save Variables unit to circumvent the lack of auto-save event hooks in WebGL";
                yield return "[Fixed] Port disconnection not adding an undo entry";
            }
        }
    }
}
