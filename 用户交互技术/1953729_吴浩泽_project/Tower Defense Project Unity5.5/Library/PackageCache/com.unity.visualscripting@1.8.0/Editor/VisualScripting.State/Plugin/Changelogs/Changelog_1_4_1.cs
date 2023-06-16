using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Changelog_1_4_1 : PluginChangelog
    {
        public Changelog_1_4_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.1";

        public override DateTime date => new DateTime(2019, 01, 22);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Changed] Allowed state machines to receive Start, OnEnable and OnDisable events for consistency";
                yield return "[Fixed] Graph data type mismatch in event listening handlers for state graphs";
                yield return "[Fixed] Non instantiated state graphs showing force enter / force exit contextual menu options";
                yield return "[Fixed] Live-added Any States not sending transitions";
                yield return "[Fixed] Any States not exiting properly when stopping the graph";
                yield return "[Fixed] Live-added start states not getting automatically entered";
                yield return "[Fixed] Force Enter and Force Exit showing in Any State context menu";
            }
        }
    }
}
