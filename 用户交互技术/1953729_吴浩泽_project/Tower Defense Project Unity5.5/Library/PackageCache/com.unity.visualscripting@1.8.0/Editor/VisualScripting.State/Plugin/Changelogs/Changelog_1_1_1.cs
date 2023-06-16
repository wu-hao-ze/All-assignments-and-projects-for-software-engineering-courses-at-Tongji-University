using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Changelog_1_1_1 : PluginChangelog
    {
        public Changelog_1_1_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.1.1";
        public override DateTime date => new DateTime(2017, 10, 10);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Changed] Default transitions to not include Update event anymore";
                yield return "[Fixed] Inactive states sometimes updating";
                yield return "[Optimized] Editor recursion performance";
            }
        }
    }
}
