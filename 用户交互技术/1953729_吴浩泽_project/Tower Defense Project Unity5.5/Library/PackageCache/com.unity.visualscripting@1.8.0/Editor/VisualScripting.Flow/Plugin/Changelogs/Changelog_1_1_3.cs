using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_1_3 : PluginChangelog
    {
        public Changelog_1_1_3(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.1.3";
        public override DateTime date => new DateTime(2017, 10, 30);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Support for reflected inspectors in literals";
                yield return "[Added] Static API shortcuts to Variables class";
                yield return "[Changed] Graph level contextual menu to Shift+RMB";
                yield return "[Optimized] Update loops";
                yield return "[Fixed] Warning unit colors missing when in play mode";
                yield return "[Fixed] Deserialization error due to nester owner being serialized";
                yield return "[Fixed] Select / Switch on integer / string failing to initialize";
            }
        }
    }
}
