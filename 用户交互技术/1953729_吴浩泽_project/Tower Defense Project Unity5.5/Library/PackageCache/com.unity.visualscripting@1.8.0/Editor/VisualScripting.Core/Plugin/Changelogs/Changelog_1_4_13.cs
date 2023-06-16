using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Changelog_1_4_13 : PluginChangelog
    {
        public Changelog_1_4_13(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.13";

        public override DateTime date => new DateTime(2020, 09, 14);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Integrated Bolt into Unity Usage Analytics";
                yield return "[Fixed] Local build machine file paths appearing in stack traces on user machines";
                yield return "[Fixed] Unity compiler warnings around deprecated code";
            }
        }
    }
}
