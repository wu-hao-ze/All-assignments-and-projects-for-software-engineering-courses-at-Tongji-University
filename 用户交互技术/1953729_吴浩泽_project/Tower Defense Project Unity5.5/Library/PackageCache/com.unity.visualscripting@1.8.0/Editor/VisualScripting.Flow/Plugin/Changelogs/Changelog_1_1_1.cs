using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_1_1 : PluginChangelog
    {
        public Changelog_1_1_1(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.1.1";
        public override DateTime date => new DateTime(2017, 10, 10);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Changed] Variable units color to teal";
                yield return "[Remove] Custom operators reflected methods";
                yield return "[Removed] Numeric Comparison unit";
                yield return "[Removed] Equality Comparison unit";
                yield return "[Added] Comparison unit";
                yield return "[Added] Generic math operator units";
                yield return "[Removed] Approximately Equal unit";
                yield return "[Removed] Approximately Not Equal unit";
                yield return "[Changed] Equal and Not Equal units handle floating point errors";
                yield return "[Added] Non-numeric mode for comparison units";
            }
        }
    }
}
