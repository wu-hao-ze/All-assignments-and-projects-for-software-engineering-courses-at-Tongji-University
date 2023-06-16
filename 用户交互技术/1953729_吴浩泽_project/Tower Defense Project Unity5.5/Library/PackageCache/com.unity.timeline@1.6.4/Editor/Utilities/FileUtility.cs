using System.IO;
using UnityEditor.VersionControl;

namespace UnityEditor.Timeline
{
    static class FileUtility
    {
        internal static bool IsReadOnly(UnityEngine.Object asset)
        {
            return IsReadOnlyImpl(asset);
        }

#if UNITY_2021_2_OR_NEWER
        static bool IsReadOnlyImpl(UnityEngine.Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(assetPath))
                return false;

            if (Provider.enabled && VersionControlUtils.IsPathVersioned(assetPath))
            {
                return !AssetDatabase.CanOpenForEdit(asset, StatusQueryOptions.UseCachedIfPossible);
            }

            return (uint)(File.GetAttributes(assetPath) & FileAttributes.ReadOnly) > 0U;
        }
#else
        static bool IsReadOnlyImpl(UnityEngine.Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (Provider.enabled)
            {
                if (!Provider.isActive)
                    return false;

                Asset vcAsset = Provider.GetAssetByPath(assetPath);
                if (Provider.IsOpenForEdit(vcAsset))
                    return false;


                //I can't get any of the Provider checks to work, but here we should check for exclusive checkout issues.
                return false;
            }


            if (!string.IsNullOrEmpty(assetPath))
            {
                return (File.GetAttributes(assetPath) & FileAttributes.ReadOnly) != 0;
            }
            return false;
        }
#endif
    }
}
