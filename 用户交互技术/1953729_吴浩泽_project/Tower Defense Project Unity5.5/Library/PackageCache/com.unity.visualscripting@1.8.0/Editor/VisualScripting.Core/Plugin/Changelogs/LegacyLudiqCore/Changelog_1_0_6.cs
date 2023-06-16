using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_0_6 : PluginChangelog
    {
        public LudiqCoreChangelog_1_0_6(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.0.6";
        public override DateTime date => new DateTime(2017, 10, 30);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Reflected inspector support for classes marked with [Inspectable]";
                yield return "[Added] AnimatorStateInfo to default types";
                yield return "[Added] Icons for Type, Assembly and AnimatorStateInfo";
                yield return "[Improved] Serialized data debug display";
                yield return "[Optimized] Plugin container initialization";
                yield return "[Optimized] Documentation parsing to background thread";
                yield return "[Optimized] Attribute reflection fetching";
                yield return "[Fixed] Missing default assemblies for Unity 2017.2+";
                yield return "[Fixed] Changes made to prefab instances not saving";
                yield return "[Fixed] Descriptor delayed update error";
                yield return "[Fixed] ExitGUIException in context menus";
                yield return "[Fixed] Reflected cloner instance creation with non-public constructors";
                yield return "[Fixed] Custom exception handling being ignored";
            }
        }
    }
}
