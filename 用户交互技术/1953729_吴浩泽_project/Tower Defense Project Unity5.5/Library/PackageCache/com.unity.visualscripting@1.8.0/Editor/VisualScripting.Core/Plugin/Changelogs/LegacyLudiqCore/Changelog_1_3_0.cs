using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqCoreChangelog_1_3_0 : PluginChangelog
    {
        public LudiqCoreChangelog_1_3_0(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.3.0";
        public override DateTime date => new DateTime(2018, 04, 06);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Changed] Folder structure";
                yield return "[Added] RenamedFrom attribute for types and members";
                yield return "[Added] Embedded loading for editor resources";
                yield return "[Refactored] Type deserialization";
                yield return "[Added] Support for .NET 4.6 compilation";
                yield return "[Removed] System.Threading dependency on .NET 4.6 builds";
                yield return "[Fixed] Invalid GUI actions whiles compiling";
                yield return "[Fixed] XML Documentation loading failure fatality";
                yield return "[Optimized] Project XML documentation loading";
                yield return "[Fixed] Deserialization failure logging fatality";
                yield return "[Added] UnityEngine.Touch to default types";
                yield return "[Fixed] FlexibleSpace and Space NullReferenceException";
                yield return "[Refactored] Decorator provider freeing";
                yield return "[Fixed] Missing valid constructors for objects derived from UnityEngine.Object";
                yield return "[Fixed] Static types being excluded in AOT safe mode";
                yield return "[Removed] SharpRaven and Newtonsoft.Json dependencies";
            }
        }
    }
}
