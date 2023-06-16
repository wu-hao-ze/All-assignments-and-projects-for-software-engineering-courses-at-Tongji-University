using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_2_2 : PluginChangelog
    {
        public LudiqCoreChangelog_1_2_2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.2.2";
        public override DateTime date => new DateTime(2017, 12, 04);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] AOT Collections";
                yield return "[Added] Adaptive width for list inspectors";
                yield return "[Added] Adaptive width for Type inspector";
                yield return "[Added] Cursor and ParticleCollisionEvent to default type options";
                yield return "[Added] Scene singleton system";
                yield return "[Improved] Error recovery in fuzzy finder";
                yield return "[Improved] Tools menu organization";
                yield return "[Improved] Singleton code";
                yield return "[Fixed] Shift number drag issue with integer field";
                yield return "[Fixed] Dictionary inspector for ordered dictionaries";
                yield return "[Fixed] AOT errors on static types";
                yield return "[Fixed] Layout errors on wizard page switching";
                yield return "[Fixed] Fuzzy finder Y position on OSX";
                yield return "[Obsoleted] Non-public members and types options";
            }
        }
    }
}
