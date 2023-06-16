using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_4_13 : PluginChangelog
    {
        public LudiqCoreChangelog_1_4_13(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.13";

        public override DateTime date => new DateTime(2020, 09, 14);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Fuzzy Finder not appearing in Flow Macro Graphs on MacOS with Unity 2020.1";
                yield return "[Fixed] NullReferenceExceptions when entering Play Mode while the Fuzzy Finder window was open";
                yield return "[Fixed] Unity compiler warnings around deprecated code";
                yield return "[Fixed] Codebase not recognizing new UnityEditor CoreModule in 2020.2";
            }
        }
    }
}
