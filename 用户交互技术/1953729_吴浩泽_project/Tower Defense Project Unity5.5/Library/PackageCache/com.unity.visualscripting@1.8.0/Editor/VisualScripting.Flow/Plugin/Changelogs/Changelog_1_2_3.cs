using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_2_3 : PluginChangelog
    {
        public Changelog_1_2_3(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.2.3";
        public override DateTime date => new DateTime(2018, 01, 25);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Issues with SceneVariable singleton";
                yield return "[Fixed] Manual events firing on idle graphs";
                yield return "[Fixed] Error in scene variable value prediction from prefab graphs";
                yield return "[Fixed] Typo in Create Dictionary node";
                yield return "[Fixed] Manual events not firing in flow machines";
            }
        }
    }
}
