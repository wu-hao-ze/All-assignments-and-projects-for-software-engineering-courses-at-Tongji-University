using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_8 : PluginChangelog
    {
        public Changelog_1_4_8(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.8";

        public override DateTime date => new DateTime(2019, 10, 28);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Memory leak in auto-stopped coroutine";
                yield return "[Fixed] Memory leak when coroutine is interrupted before disposal of preserved flow stack";
            }
        }
    }

    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_8f2 : PluginChangelog
    {
        public Changelog_1_4_8f2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.8f2";

        public override DateTime date => new DateTime(2019, 10, 31);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Error when using coroutines in state transition caused by overly aggressive memory leak fix";
            }
        }
    }
}
