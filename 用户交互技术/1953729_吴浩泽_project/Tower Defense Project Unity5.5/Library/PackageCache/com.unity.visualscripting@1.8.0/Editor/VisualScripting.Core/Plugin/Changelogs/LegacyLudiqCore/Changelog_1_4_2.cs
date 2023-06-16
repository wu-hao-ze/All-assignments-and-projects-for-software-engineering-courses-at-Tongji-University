using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_4_2 : PluginChangelog
    {
        public LudiqCoreChangelog_1_4_2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.2";

        public override DateTime date => new DateTime(2019, 04, 03);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Fix GetCorrespondingObjectFromSource failing to reflect on Unity 2018.3";
            }
        }
    }
}
