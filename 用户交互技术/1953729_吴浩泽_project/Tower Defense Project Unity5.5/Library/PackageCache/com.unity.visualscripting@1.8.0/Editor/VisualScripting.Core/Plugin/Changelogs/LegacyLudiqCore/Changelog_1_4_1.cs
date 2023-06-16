using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_4_1 : PluginChangelog
    {
        public LudiqCoreChangelog_1_4_1(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.1";

        public override DateTime date => new DateTime(2019, 01, 22);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Various errors when deserializing Unity objects with the new prefab workflow";
                yield return "[Fixed] AudioMixerController and AudioMixerGroupController references being lost on build";
                yield return "[Fixed] AOT Safe Mode filtering member return types";
                yield return "[Fixed] Error recovery when RenamedFrom attributes fail to fetch";
                yield return "[Fixed] XML documentation generator with latest MSBuild version in Unity 2018.3";
                yield return "[Fixed] Improved error recovery in AOT Pre-Builder when scenes in build settings are not found";
                yield return "[Fixed] Serialization callbacks being sent to object references";
            }
        }
    }
}
