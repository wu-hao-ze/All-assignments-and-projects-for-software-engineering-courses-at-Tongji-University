using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_4_11 : PluginChangelog
    {
        public LudiqCoreChangelog_1_4_11(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.11";

        public override DateTime date => new DateTime(2020, 01, 25);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Codebase failing to initialize when assembly metadata is corrupted (often due to obfuscation)";
                yield return "[Fixed] False positive detection of out parameter modifier when parameter was marked with [Out] attribute";
            }
        }
    }
}
