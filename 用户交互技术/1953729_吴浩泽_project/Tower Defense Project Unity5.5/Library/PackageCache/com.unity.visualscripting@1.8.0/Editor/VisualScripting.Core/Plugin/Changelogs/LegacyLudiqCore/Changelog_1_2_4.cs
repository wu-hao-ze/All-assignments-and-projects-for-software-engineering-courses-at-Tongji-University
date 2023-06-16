using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_2_4 : PluginChangelog
    {
        public LudiqCoreChangelog_1_2_4(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.2.4";
        public override DateTime date => new DateTime(2018, 02, 26);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Serialized object references list changing on undo stack";
                yield return "[Fixed] Fuzzy finder keyboard focus on Linux";
                yield return "[Fixed] Missing list, interface and generic options on JIT platforms";
                yield return "[Fixed] AnimationCurve disappearing in play mode on macro graphs";
                yield return "[Fixed] Harmless fuzzy window error and warnings";
                yield return "[Fixed] Singleton initialization order issues";
            }
        }
    }
}
