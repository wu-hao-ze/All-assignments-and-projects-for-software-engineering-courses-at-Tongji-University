using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_0_4 : PluginChangelog
    {
        public LudiqCoreChangelog_1_0_4(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.0.4";
        public override DateTime date => new DateTime(2017, 10, 10);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Boolean inspector height";
                yield return "[Fixed] Unity Object inspector adaptive width";
                yield return "[Fixed] Equality and inequality handling for numeric types and nulls";
            }
        }
    }
}
