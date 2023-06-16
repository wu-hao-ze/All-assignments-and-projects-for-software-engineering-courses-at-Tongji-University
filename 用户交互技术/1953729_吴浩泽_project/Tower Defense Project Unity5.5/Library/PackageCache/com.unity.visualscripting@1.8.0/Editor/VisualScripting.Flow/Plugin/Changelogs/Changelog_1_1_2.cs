using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_1_2 : PluginChangelog
    {
        public Changelog_1_1_2(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.1.2";
        public override DateTime date => new DateTime(2017, 10, 16);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Wait units";
                yield return "[Added] Animation events";
                yield return "[Added] UnityEvent event";
                yield return "[Added] Smart contextual options for numeric and boolean input ports";
                yield return "[Optimized] Super unit memory allocation";
                yield return "[Optimized] Member invocation units memory allocation";
                yield return "[Optimized] Loop units memory allocation";
            }
        }
    }
}
