#pragma warning disable 162

using System.IO;
using UnityEngine;

namespace Unity.VisualScripting
{
    [PluginModule(required = true)]
    public class PluginPaths : IPluginModule
    {
        protected PluginPaths(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public virtual void Initialize()
        {
            if (isFirstPass)
            {
                Debug.LogWarning($"Plugin '{plugin.id}' is in a special folder that makes it compile first.\nThis might cause issues with generated assets.");
            }
        }

        public virtual void LateInitialize() { }

        public Plugin plugin { get; }

        private static string _package;

        // Note: The Bolt 'package' can exist within the Assets folder OR within the Packages folder in a project, depending on
        // when we release Bolt as a package / what the user has set up in an existing project.
        public static string package
        {
            get
            {
                if (_package == null)
                {
                    _package = PathUtility.GetPackageRootPath();
                }

                return _package;
            }
        }

        public const string FOLDER_BOLT_GENERATED = "Unity.VisualScripting.Generated";
        public static string ASSETS_FOLDER_BOLT_GENERATED = Path.Combine("Assets", FOLDER_BOLT_GENERATED);

        public bool isFirstPass => package.Contains("/Plugins/") || package.Contains("/Standard Assets/") || package.Contains("/Pro Standard Assets/");

        public static string resourcesFolder => Path.Combine(package, $"Editor/VisualScripting.Core/EditorAssetResources");

        internal const string assetBundleRoot = "Assets/EditorResources";

        internal const string assetBundle = "visualscripting.editor_assets";

        public static string resourcesBundle = Path.Combine(package, $"Editor/VisualScripting.Core/EditorAssetResources/{assetBundle}");

        public static string generated => Path.Combine(Paths.assets, FOLDER_BOLT_GENERATED);

        public string persistentGenerated => Path.Combine(generated, plugin.id);

        public string transientGenerated => Path.Combine(generated, plugin.id);

        public static string projectSettings => Path.Combine(Paths.project, "ProjectSettings", "VisualScriptingSettings.asset");
    }
}
