using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_4_8 : PluginChangelog
    {
        public LudiqCoreChangelog_1_4_8(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.8";

        public override DateTime date => new DateTime(2019, 10, 28);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Memory leak in recursion class";
                yield return "[Fixed] Memory leak caused by reflection helpers holding on to its target past their lifetime";
                yield return "[Fixed] Version control utility failing to checkout files off main thread";
                yield return "[Fixed] Memory leak caused by assets never unloading";
                yield return "[Changed] Added fallback to file checkout to force writable and warn";
            }
        }
    }
}
