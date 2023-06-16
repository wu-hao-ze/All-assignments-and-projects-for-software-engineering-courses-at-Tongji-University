using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    internal class Changelog_1_1_3 : PluginChangelog
    {
        public Changelog_1_1_3(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.1.3";
        public override DateTime date => new DateTime(2017, 10, 30);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Deserialization error due to nester owner being serialized";
                yield return "[Fixed] Descriptor error with nested events";
                yield return "[Fixed] Event listening state being serialized";
            }
        }
    }
}
