using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace Unity.VisualScripting
{
    [Product(ID)]
    public sealed class BoltProduct : Product
    {
        public BoltProduct() { }

        public override void Initialize()
        {
            base.Initialize();

            logo = BoltCore.Resources.LoadTexture("LogoBolt.png", CreateTextureOptions.Scalable)?.Single();
        }

        public override string configurationPanelLabel => "Bolt";

        public override string name => "Bolt";
        public override string description => "";
        public override string authorLabel => "Designed & Developed by ";
        public override string author => "";
        public override string copyrightHolder => "Unity";
        public override SemanticVersion version => PackageVersionUtility.version;
        public override string publisherUrl => "";
        public override string websiteUrl => "";
        public override string supportUrl => "";
        public override string manualUrl => "https://docs.unity3d.com/Packages/com.unity.bolt@latest";
        public override string assetStoreUrl => "http://u3d.as/1Md2";

        public const string ID = "Bolt";

#if VISUAL_SCRIPT_INTERNAL
        public const int ToolsMenuPriority = -990000;
        public const int DeveloperToolsMenuPriority = ToolsMenuPriority + 1000;
#endif

        public static BoltProduct instance => (BoltProduct)ProductContainer.GetProduct(ID);

        private static bool PrepareForRelease()
        {
            if (!EditorUtility.DisplayDialog("Delete Generated Files", "This action will delete all generated files, including those containing user data.\n\nAre you sure you want to continue?", "Confirm", "Cancel"))
            {
                return false;
            }

            PluginConfiguration.DeleteAllProjectSettings();

            foreach (var plugin in PluginContainer.plugins)
            {
                PathUtility.DeleteDirectoryIfExists(plugin.paths.persistentGenerated);
                PathUtility.DeleteDirectoryIfExists(plugin.paths.transientGenerated);
            }

            return true;
        }

#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Export Release Asset Package...", priority = DeveloperToolsMenuPriority + 102)]
#endif
        private static void ExportReleasePackage()
        {
            if (!PrepareForRelease())
            {
                return;
            }

            var exportPath = EditorUtility.SaveFilePanel("Export Release Package",
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "Bolt_" + instance.version.ToString().Replace(".", "_").Replace(" ", "_"),
                "unitypackage");

            if (exportPath == null)
            {
                return;
            }

            var packageDirectory = Path.GetDirectoryName(exportPath);

            var paths = new List<string>()
            {
                PathUtility.GetPackageRootPath()
            };

            AssetDatabase.ExportPackage(paths.ToArray(), exportPath, ExportPackageOptions.Recurse);

            if (EditorUtility.DisplayDialog("Export Release Package", "Release package export complete.\nOpen containing folder?", "Open Folder", "Close"))
            {
                Process.Start(packageDirectory);
            }
        }
    }
}
