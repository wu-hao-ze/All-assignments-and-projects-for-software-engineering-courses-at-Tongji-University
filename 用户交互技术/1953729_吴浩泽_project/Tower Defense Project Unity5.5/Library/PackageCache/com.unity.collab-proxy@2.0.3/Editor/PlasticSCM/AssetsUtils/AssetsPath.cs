using System.IO;
using System.Reflection;

using UnityEditor;
using UnityEngine;

using Codice.Client.Common;
using Codice.Utils;
using PlasticGui;

namespace Unity.PlasticSCM.Editor.AssetUtils
{
    internal static class AssetsPath
    {
        internal static class GetFullPath
        {
            internal static string ForObject(Object obj)
            {
                string relativePath = AssetDatabase.GetAssetPath(obj);

                if (string.IsNullOrEmpty(relativePath))
                    return null;

                return Path.GetFullPath(relativePath);
            }

            internal static string ForGuid(string guid)
            {
                string relativePath = GetAssetPath(guid);

                if (string.IsNullOrEmpty(relativePath))
                    return null;

                return Path.GetFullPath(relativePath);
            }
        }

        internal static class GetFullPathUnderWorkspace
        {
            internal static string ForAsset(
                string wkPath,
                string assetPath)
            {
                if (string.IsNullOrEmpty(assetPath))
                    return null;

                string fullPath = Path.GetFullPath(assetPath);

                if (!PathHelper.IsContainedOn(fullPath, wkPath))
                    return null;

                return fullPath;
            }

            internal static string ForGuid(
                string wkPath,
                string guid)
            {
                return ForAsset(wkPath, GetAssetPath(guid));
            }
        }

        internal static string GetLayoutsFolderRelativePath()
        {
            return string.Concat(mAssetsFolderLocation, "/Layouts");
        }

        internal static string GetStylesFolderRelativePath()
        {
            return string.Concat(mAssetsFolderLocation, "/Styles");
        }

        internal static string GetImagesFolderRelativePath()
        {
            return string.Concat(mAssetsFolderLocation, "/Images");
        }

        internal static string GetRelativePath(string fullPath)
        {
            return PathHelper.GetRelativePath(
                mProjectFullPath, fullPath).Substring(1);
        }

        internal static bool IsRunningAsUPMPackage()
        {
            string unityPlasticDllPath = Path.GetFullPath(
                AssemblyLocation.GetAssemblyDirectory(
                    Assembly.GetAssembly(typeof(PlasticLocalization))));

            return Directory.Exists(
                Path.GetFullPath(Path.Combine(
                    unityPlasticDllPath,
                    // assets relative path when running as a UPM package
                    "../../../Editor/PlasticSCM/Assets")));
        }

        static string GetAssetPath(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;

            return AssetDatabase.GUIDToAssetPath(guid);
        }

        static AssetsPath()
        {
            mAssetsFolderLocation = (IsRunningAsUPMPackage()) ?
                "Packages/com.unity.collab-proxy/Editor/PlasticSCM/Assets" :
                "Assets/Plugins/PlasticSCM/Editor/Assets";
        }

        static string mProjectFullPath = ProjectPath.
            FromApplicationDataPath(ApplicationDataPath.Get());

        static string mAssetsFolderLocation;
    }
}
