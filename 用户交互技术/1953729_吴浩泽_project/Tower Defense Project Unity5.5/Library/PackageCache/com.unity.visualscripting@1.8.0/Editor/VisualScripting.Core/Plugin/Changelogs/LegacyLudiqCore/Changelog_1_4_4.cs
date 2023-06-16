using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_4_4 : PluginChangelog
    {
        public LudiqCoreChangelog_1_4_4(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.4";

        public override DateTime date => new DateTime(2019, 06, 11);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Documentation generator MSBuild path on Visual Studio 2019";
                yield return "[Fixed] Force locked assembly reloads during play mode to avoid data corruption";
            }
        }
    }
}
