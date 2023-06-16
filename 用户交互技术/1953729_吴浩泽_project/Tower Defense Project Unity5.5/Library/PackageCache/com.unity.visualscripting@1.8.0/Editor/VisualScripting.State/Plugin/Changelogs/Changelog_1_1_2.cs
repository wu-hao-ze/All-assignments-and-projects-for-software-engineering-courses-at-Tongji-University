using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Changelog_1_1_2 : PluginChangelog
    {
        public Changelog_1_1_2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.1.2";
        public override DateTime date => new DateTime(2017, 10, 16);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Issue with dragging";
            }
        }
    }
}
