using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_2_1 : PluginChangelog
    {
        public LudiqCoreChangelog_1_2_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.2.1";
        public override DateTime date => new DateTime(2017, 11, 22);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Flags enum inspector and serialization";
                yield return "[Fixed] Issue with holding Shift in text fields";
                yield return "[Fixed] Error in fuzzy finder search";
                yield return "[Fixed] User-defined conversion operators not generating AOT stubs";
                yield return "[Fixed] System.Threading dependency issue";
                yield return "[Fixed] Error from Mono GetCustomAttributes method";
                yield return "[Fixed] Exception handling propagation";
            }
        }
    }
}
