using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_2_3 : PluginChangelog
    {
        public LudiqCoreChangelog_1_2_3(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.2.3";
        public override DateTime date => new DateTime(2018, 01, 25);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] New Alpha and Beta systems";
                yield return "[Added] Screen to default types with icon";
                yield return "[Added] Warning for object reference order changing after undo";
                yield return "[Changed] Hide interface methods in AOT safe-mode";
                yield return "[Fixed] Human naming error on MemberInfo";
                yield return "[Fixed] AOT Pre-Build not including stubs from disabled game objects";
                yield return "[Fixed] Unity Object to interface conversion";
                yield return "[Fixed] Virtual method overrides not being used in IL2CPP";
                yield return "[Fixed] Number to string units on IL2CPP";
                yield return "[Fixed] Unity hierarchy conversion issue with pseudo-nulls";
                yield return "[Fixed] AOT Pre-Build not including stubs from uninstantiated prefabs";
            }
        }
    }
}
