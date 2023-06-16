using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(ID)]
    [PluginDependency(BoltCore.ID)]
    [Product(BoltProduct.ID)]
    [PluginRuntimeAssembly("Unity." + ID)]
    public sealed class BoltState : Plugin
    {
        [RenamedFrom("Bolt.State")]
        public const string ID = "VisualScripting.State";

        public BoltState() : base()
        {
            instance = this;
        }

        public static BoltState instance { get; private set; }

        public static BoltStateManifest Manifest => (BoltStateManifest)instance?.manifest;
        public static BoltStateConfiguration Configuration => (BoltStateConfiguration)instance?.configuration;
        public static BoltStateResources Resources => (BoltStateResources)instance?.resources;
        public static BoltStateResources.Icons Icons => Resources?.icons;
        public const string LegacyRuntimeDllGuid = "dcd2196c4e9166f499793f2007fcda35";
        public const string LegacyEditorDllGuid = "25cf173c22a896d44ae550407b10ed98";

        public override IEnumerable<ScriptReferenceReplacement> scriptReferenceReplacements
        {
            get
            {
#pragma warning disable 618
                yield return ScriptReferenceReplacement.From<StateMachine>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "StateMachine"));
                yield return ScriptReferenceReplacement.From<StateGraphAsset>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "StateMacro"));
#pragma warning restore 618
            }
        }
    }
}
