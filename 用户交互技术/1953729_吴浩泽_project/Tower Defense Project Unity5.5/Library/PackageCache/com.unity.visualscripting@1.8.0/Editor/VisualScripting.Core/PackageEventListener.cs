#if UNITY_2020_2_OR_NEWER
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Unity.VisualScripting
{
    // See https://confluence.unity3d.com/pages/viewpage.action?spaceKey=PAK&title=How+to+subscribe+to+the+package+manager+events for more info
    [UsedImplicitly]
    public sealed class PackageEventListener
    {
        internal static void SubscribeToEvents()
        {
            Events.registeringPackages += OnRegisteringPackages;
        }

        static void OnRegisteringPackages(PackageRegistrationEventArgs args)
        {
            foreach (var removedPackage in args.removed)
            {
                if (removedPackage.name == "com.unity.visualscripting")
                {
                    if (AssetDatabase.IsValidFolder(PluginPaths.ASSETS_FOLDER_BOLT_GENERATED))
                    {
                        FileUtil.DeleteFileOrDirectory(PluginPaths.ASSETS_FOLDER_BOLT_GENERATED);
                        FileUtil.DeleteFileOrDirectory($"{PluginPaths.ASSETS_FOLDER_BOLT_GENERATED}.meta");
                    }
                }
            }
        }
    }
}
#endif
