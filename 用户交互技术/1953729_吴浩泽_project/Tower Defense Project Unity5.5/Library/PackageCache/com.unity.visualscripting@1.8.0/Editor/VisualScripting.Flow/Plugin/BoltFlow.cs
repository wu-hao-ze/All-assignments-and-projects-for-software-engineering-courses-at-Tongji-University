using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(ID)]
    [PluginDependency(BoltCore.ID)]
    [Product(BoltProduct.ID)]
    [PluginRuntimeAssembly("Unity." + ID)]
    public sealed class BoltFlow : Plugin
    {
        public BoltFlow()
        {
            instance = this;
        }

        public static BoltFlow instance { get; private set; }

        [RenamedFrom("Bolt.Flow")]
        public const string ID = "VisualScripting.Flow";

        public static BoltFlowManifest Manifest => (BoltFlowManifest)instance?.manifest;

        public static BoltFlowConfiguration Configuration => (BoltFlowConfiguration)instance?.configuration;

        public static BoltFlowResources Resources => (BoltFlowResources)instance?.resources;

        public static BoltFlowResources.Icons Icons => Resources?.icons;

        public static BoltFlowPaths Paths => (BoltFlowPaths)instance?.paths;

        public const string LegacyRuntimeDllGuid = "a040fb66244a7f54289914d98ea4ef7d";

        public const string LegacyEditorDllGuid = "6cb65bfc2ee1c854ca1382175f3aba91";

        public override IEnumerable<ScriptReferenceReplacement> scriptReferenceReplacements
        {
            get
            {
#pragma warning disable 618
                yield return ScriptReferenceReplacement.From<ScriptMachine>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "FlowMachine"));
                yield return ScriptReferenceReplacement.From<ScriptGraphAsset>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "FlowMacro"));
                // Variables moved to Bolt.Core assembly in v.1.3
                yield return ScriptReferenceReplacement.From<Variables>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "Variables"));
                yield return ScriptReferenceReplacement.From<SceneVariables>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "SceneVariables"));
                yield return ScriptReferenceReplacement.From<VariablesAsset>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "VariablesAsset"));
#pragma warning restore 618
            }
        }

        public override IEnumerable<string> tips
        {
            get
            {
                yield return "Did you know you can dance?";
                yield return "Lorem ipsum dolor sit amet";
            }
        }

        public override void RunAction()
        {
            UnitBase.Build(true);
        }
    }
}
