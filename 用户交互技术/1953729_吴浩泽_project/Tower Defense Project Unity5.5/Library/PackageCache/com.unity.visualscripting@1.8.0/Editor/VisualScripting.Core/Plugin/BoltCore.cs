using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[assembly: Unity.VisualScripting.RenamedAssembly("Bolt.Core.Editor", "Unity.VisualScripting.Core.Editor")]
[assembly: Unity.VisualScripting.RenamedAssembly("Bolt.Core.Runtime", "Unity.VisualScripting.Core")]
[assembly: Unity.VisualScripting.RenamedAssembly("Bolt.Flow.Editor", "Unity.VisualScripting.Flow.Editor")]
[assembly: Unity.VisualScripting.RenamedAssembly("Bolt.Flow.Runtime", "Unity.VisualScripting.Flow")]
[assembly: Unity.VisualScripting.RenamedAssembly("Bolt.State.Editor", "Unity.VisualScripting.State.Editor")]
[assembly: Unity.VisualScripting.RenamedAssembly("Bolt.State.Runtime", "Unity.VisualScripting.State")]
[assembly: Unity.VisualScripting.RenamedAssembly("Ludiq.Core.Editor", "Unity.VisualScripting.Core.Editor")]
[assembly: Unity.VisualScripting.RenamedAssembly("Ludiq.Core.Runtime", "Unity.VisualScripting.Core")]
[assembly: Unity.VisualScripting.RenamedAssembly("Ludiq.Graphs.Editor", "Unity.VisualScripting.Core.Editor")]
[assembly: Unity.VisualScripting.RenamedAssembly("Ludiq.Graphs.Runtime", "Unity.VisualScripting.Core")]

namespace Unity.VisualScripting
{
    [Plugin(ID)]
    [Product(BoltProduct.ID)]
    [PluginRuntimeAssembly("Unity." + ID)]
    public sealed class BoltCore : Plugin
    {
        [RenamedFrom("Bolt.Core")]
        [RenamedFrom("Ludiq.Core")]
        [RenamedFrom("Ludiq.Graphs")]
        public const string ID = "VisualScripting.Core";

        public BoltCore() : base()
        {
            instance = this;
        }

        public static BoltCore instance { get; private set; }

        public static BoltCoreManifest Manifest => (BoltCoreManifest)instance?.manifest;
        public static BoltCoreConfiguration Configuration => (BoltCoreConfiguration)instance?.configuration;
        public static BoltCoreResources Resources => (BoltCoreResources)instance?.resources;
        public static BoltCorePaths Paths => (BoltCorePaths)instance?.paths;
        public static BoltCoreResources.Icons Icons => Resources?.icons;

        public const string LegacyRuntimeDllGuid = "c8d0ad23af520fe46aabe2b1fecf6462";

        public const string LegacyEditorDllGuid = "7314928a14330c04fb980214791646e9";

        public const string LegacyLudiqCoreRuntimeDllGuid = "1eea3bf15bb7ddb4582c462beee0ad13";
        public const string LegacyLudiqCoreEditorDllGuid = "8878d90c345be1a43ab0c9a9898ad433";

        public override IEnumerable<ScriptReferenceReplacement> scriptReferenceReplacements
        {
            get
            {
#pragma warning disable 618
                yield return ScriptReferenceReplacement.From<Variables>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "Variables"));
                yield return ScriptReferenceReplacement.From<VariablesAsset>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "VariablesAsset"));
                yield return ScriptReferenceReplacement.From<VariablesSaver>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "VariablesSaver"));
                yield return ScriptReferenceReplacement.From<SceneVariables>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Bolt", "SceneVariables"));

                yield return ScriptReferenceReplacement.From<DictionaryAsset>(ScriptReference.Dll(LegacyLudiqCoreRuntimeDllGuid, "Ludiq", "DictionaryAsset"));
#pragma warning restore 618
            }
        }

        public static class Styles
        {
            static Styles()
            {
                nodeLabel = new GUIStyle(EditorStyles.label);

                if (EditorGUIUtility.isProSkin)
                {
                    nodeLabel.normal.textColor = ColorUtility.Gray(0.92f);
                }
            }

            public static readonly GUIStyle nodeLabel;
        }

        public override IEnumerable<Page> SetupWizardPages()
        {
            yield return new NamingSchemePage();

            // Disabling for now as they have too high a risk for failure
            // (especially GenerateDocumentation) and scare off new users.
            // The operations remain available in the menu.
            // yield return new GenerateDocumentationPage();
            // yield return new GeneratePropertyProvidersPage();
        }
    }
}
