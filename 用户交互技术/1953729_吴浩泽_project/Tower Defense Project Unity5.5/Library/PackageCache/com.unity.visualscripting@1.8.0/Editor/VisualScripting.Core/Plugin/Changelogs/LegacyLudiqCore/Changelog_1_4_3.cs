using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_4_3 : PluginChangelog
    {
        public LudiqCoreChangelog_1_4_3(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.3";

        public override DateTime date => new DateTime(2019, 04, 29);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Issue with Unity 2019 compatibility due to internal annotations API change";
                yield return "[Fixed] Standalone JIT support detection";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_4_3f2 : PluginChangelog
    {
        public LudiqCoreChangelog_1_4_3f2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.3f2";

        public override DateTime date => new DateTime(2019, 05, 02);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Temporarily disabled JIT on Standalone + Mono platforms as they throw PlatformNotSupportedException";
            }
        }
    }
}
