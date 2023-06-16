using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_1 : PluginChangelog
    {
        public Changelog_1_4_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.1";

        public override DateTime date => new DateTime(2019, 01, 22);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Reverted Formula unit caching behaviour to 1.4.0f10";
                yield return "[Added] Cache arguments option to Formula unit";
                yield return "[Fixed] Issue where automated coroutine stop would error if it exited during its first frame";
                yield return "[Added] Warning when a unit connection fails to get created during deserialization";
                yield return "[Fixed] Bug where interned graph reference with destroyed root object would match alive reference when exiting play mode";
                yield return "[Fixed] Issue where scene variable prediction would try to instantiate infinite scene variable singletons in prefab isolation stage";
                yield return "[Fixed] Improved error recovery in AOT stubs lookup for Expose unit";
            }
        }
    }
}
