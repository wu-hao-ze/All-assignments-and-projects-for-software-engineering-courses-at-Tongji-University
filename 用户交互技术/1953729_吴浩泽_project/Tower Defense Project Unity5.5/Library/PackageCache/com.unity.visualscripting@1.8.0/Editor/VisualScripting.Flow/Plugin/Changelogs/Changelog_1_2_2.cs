using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_2_2 : PluginChangelog
    {
        public Changelog_1_2_2(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.2.2";
        public override DateTime date => new DateTime(2017, 12, 04);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Incremental unit database update";
                yield return "[Added] Naming-scheme hot switching";
                yield return "[Added] Isolated scene variables";
                yield return "[Added] Option to disable automatic scene variables creation";
                yield return "[Added] List Contains Item unit";
                yield return "[Added] Dictionary Contains Key unit";
                yield return "[Changed] Scene variables API";
                yield return "[Changed] Moved naming scheme from unit options wizard to editor preferences";
                yield return "[Removed] Member setting on codebase reflected units (use replacement instead";
                yield return "[Fixed] Drag & drop of scene-bound components";
                yield return "[Fixed] Error in unit warnings for destroyed objects";
                yield return "[Fixed] Is Variable Defined unit creation from fuzzy finder";
                yield return "[Fixed] Fuzzy finder error when parent components are missing";
                yield return "[Fixed] Game object events fetching target twice";
            }
        }
    }
}
