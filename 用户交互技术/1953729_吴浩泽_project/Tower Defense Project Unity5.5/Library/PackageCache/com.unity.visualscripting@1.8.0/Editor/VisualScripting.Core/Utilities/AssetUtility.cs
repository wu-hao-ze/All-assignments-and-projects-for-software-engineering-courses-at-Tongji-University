using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class AssetUtility
    {
        private static AssetBundle assetBundleEditor;

        public static AssetBundle LoadAssetBundle(string name, string path)
        {
            foreach (AssetBundle assetBundle in AssetBundle.GetAllLoadedAssetBundles())
            {
                if (assetBundle.name == name)
                {
                    assetBundle.Unload(true);

                    break;
                }
            }

            return AssetBundle.LoadFromFile(path);
        }
        public static AssetBundle AssetBundleEditor
        {
            get
            {
                if (assetBundleEditor == null)
                {
                    assetBundleEditor = LoadAssetBundle(PluginPaths.assetBundle, PluginPaths.resourcesBundle);
                }

                return assetBundleEditor;
            }
        }

        public static IEnumerable<T> GetAllAssetsOfType<T>()
        {
            if (typeof(UnityObject).IsAssignableFrom(typeof(T)))
            {
                return AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadMainAssetAtPath)
                    .OfType<T>();
            }
            else
            {
                // GetAllAssetPaths is undocumented and sometimes returns
                // paths that are outside the assets folder, hence the where filter.
                var result = AssetDatabase.GetAllAssetPaths()
                    .Where(p => p.StartsWith("Assets"))
                    .Select(AssetDatabase.LoadMainAssetAtPath)
                    .OfType<T>();

                EditorUtility.UnloadUnusedAssetsImmediate();
                return result;
            }
        }

        public static string GetSelectedFolderPath()
        {
            foreach (UnityObject uo in Selection.GetFiltered(typeof(UnityObject), SelectionMode.Assets))
            {
                var assetPath = AssetDatabase.GetAssetPath(uo);

                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    return Path.Combine(Paths.project, assetPath);
                }
            }

            return null;
        }

        public static int GetFileID(string @namespace, string name)
        {
            // Gets the fileID of a given type inside a DLL asset.
            // "Given a type t, the fileID is equal to the first four bytes of the MD4 of the string `"s\0\0\0" + t.Namespace + t.Name' as a little endian 32-byte integer."
            // https://forum.unity.com/threads/yaml-fileid-hash-function-for-dll-scripts.252075/#post-1695479

            string toBeHashed = "s\0\0\0" + @namespace + name;

            using (var hash = new MD4())
            {
                byte[] hashed = hash.ComputeHash(Encoding.UTF8.GetBytes(toBeHashed));

                int result = 0;

                for (int i = 3; i >= 0; --i)
                {
                    result <<= 8;
                    result |= hashed[i];
                }

                return result;
            }
        }

        public static int GetFileID(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            return GetFileID(type.Namespace, type.Name);
        }

        public static string GetPluginRuntimeGUID(Plugin plugin)
        {
            Ensure.That(nameof(plugin)).IsNotNull(plugin);

            return AssetDatabase.AssetPathToGUID(PathUtility.FromProject(plugin.runtimeAssembly.Location));
        }

        public static bool TryLoadIfExists<T>(string path, out T asset) where T : ScriptableObject
        {
            var assetDatabasePath = PathUtility.FromProject(path);

            if (File.Exists(path))
            {
                // Try loading the existing asset file.
                asset = AssetDatabase.LoadAssetAtPath<T>(assetDatabasePath);

                if (asset == null)
                {
                    // The file exists, but it isn't a valid asset.
                    // Warn and leave the asset as is to prevent losing its serialized contents
                    // because we might be able to salvage them by deserializing later on.
                    // Return a new empty instance in the mean time.
                    Debug.LogWarning($"Loading {typeof(T).FullName} failed:\n{assetDatabasePath}");
                    asset = ScriptableObject.CreateInstance<T>();
                    return false;
                }

                return true;
            }

            asset = default;
            return false;
        }

        public static bool TryLoad<T>(string path, out T asset) where T : ScriptableObject
        {
            var assetDatabasePath = PathUtility.FromProject(path);

            if (File.Exists(path))
            {
                // Try loading the existing asset file.
                asset = AssetDatabase.LoadAssetAtPath<T>(assetDatabasePath);

                if (asset == null)
                {
                    // The file exists, but it isn't a valid asset.
                    // Warn and leave the asset as is to prevent losing its serialized contents
                    // because we might be able to salvage them by deserializing later on.
                    // Return a new empty instance in the mean time.
                    Debug.LogWarning($"Loading {typeof(T).FullName} failed:\n{assetDatabasePath}");
                    asset = ScriptableObject.CreateInstance<T>();
                    return false;
                }
            }
            else
            {
                // The file doesn't exist, so create a new asset and save it.
                asset = ScriptableObject.CreateInstance<T>();
                PathUtility.CreateParentDirectoryIfNeeded(path);
                AssetDatabase.CreateAsset(asset, assetDatabasePath);
                AssetDatabase.SaveAssets();
            }

            return true;
        }
    }
}
