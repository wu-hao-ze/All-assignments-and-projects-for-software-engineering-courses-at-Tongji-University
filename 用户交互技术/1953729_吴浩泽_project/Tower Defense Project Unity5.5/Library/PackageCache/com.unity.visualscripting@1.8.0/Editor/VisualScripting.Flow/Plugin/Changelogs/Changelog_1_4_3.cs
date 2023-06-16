using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_3 : PluginChangelog
    {
        public Changelog_1_4_3(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.3";

        public override DateTime date => new DateTime(2019, 04, 29);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Issue where literal widget failed to render when literal type had failed to deserialize";
                yield return "[Fixed] Unit header text rendering by disabling word wrapping";
            }
        }
    }
}
