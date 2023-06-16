using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Changelog_1_4_1 : PluginChangelog
    {
        public Changelog_1_4_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.1";

        public override DateTime date => new DateTime(2019, 01, 22);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Changed] Clarified variable tab labels for prefabs in the Object Variables label";
                yield return "[Changed] Inverted the order of Prefab Instance and Prefab Definition tabs in the Object Variables window";
                yield return "[Fixed] Live added graph elements starting to listen on disabled machines";
                yield return "[Fixed] Instantiation events not being sent when enabled machine changed its graph";
            }
        }
    }
}
