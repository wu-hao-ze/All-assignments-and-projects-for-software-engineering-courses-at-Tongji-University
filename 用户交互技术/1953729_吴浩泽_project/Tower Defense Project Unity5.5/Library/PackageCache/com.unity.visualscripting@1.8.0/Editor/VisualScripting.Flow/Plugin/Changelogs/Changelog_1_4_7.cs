using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_7 : PluginChangelog
    {
        public Changelog_1_4_7(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.7";

        public override DateTime date => new DateTime(2019, 09, 26);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Graph stack error when Timer or Cooldown exited super units";
                yield return "[Optimized] Unit options building by displaying fewer progress bar updates";
            }
        }
    }
}
