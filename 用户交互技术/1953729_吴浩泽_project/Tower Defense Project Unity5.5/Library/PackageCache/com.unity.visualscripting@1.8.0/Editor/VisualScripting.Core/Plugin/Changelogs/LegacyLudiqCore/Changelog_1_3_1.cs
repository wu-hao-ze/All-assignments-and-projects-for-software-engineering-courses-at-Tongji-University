using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_3_1 : PluginChangelog
    {
        public LudiqCoreChangelog_1_3_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.3.1";
        public override DateTime date => new DateTime(2018, 05, 02);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Prefabs not affected by ScriptReferenceResolver";
                yield return "[Fixed] List inspector not drawing on Unity 2018.2+";
            }
        }
    }
}
