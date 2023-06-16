using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class ScriptUtility
    {
        private static HashSet<string> guids;

        private static Dictionary<string, DateTime> guidTimestamps;

        private static Dictionary<Type, HashSet<string>> typesToGuids;

        private static Dictionary<string, HashSet<Type>> guidsToTypes;

        private static bool analyzed;

        private static void EnsureAnalyzed()
        {
            if (!analyzed)
            {
                Analyze();
            }
        }

        private static void Analyze()
        {
            UnityAPI.AwaitForever
                (
                    () =>
                    {
                        guids = new HashSet<string>();
                        guidTimestamps = new Dictionary<string, DateTime>();
                        typesToGuids = new Dictionary<Type, HashSet<string>>();
                        guidsToTypes = new Dictionary<string, HashSet<Type>>();

                        foreach (var script in Resources.FindObjectsOfTypeAll<MonoScript>())
                        {
                            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(script, out var guid, out long localId))
                            {
                                Debug.LogWarning($"Failed to get GUID for script {script.GetClass()}: {script}");
                                continue;
                            }

                            guids.Add(guid);

                            var type = script.GetClass();

                            if (!guidsToTypes.ContainsKey(guid))
                            {
                                guidsToTypes.Add(guid, new HashSet<Type>());
                            }

                            if (!guidTimestamps.ContainsKey(guid))
                            {
                                var path = AssetDatabase.GetAssetPath(script);

                                if (path.StartsWith("Assets"))
                                {
                                    path = Path.Combine(Paths.project, path);
                                }

                                var timestamp = File.GetLastWriteTimeUtc(path);
                                guidTimestamps.Add(guid, timestamp);
                            }

                            if (type != null)
                            {
                                if (!typesToGuids.ContainsKey(type))
                                {
                                    typesToGuids.Add(type, new HashSet<string>());
                                }

                                typesToGuids[type].Add(guid);
                                guidsToTypes[guid].Add(type);
                            }
                        }

                        analyzed = true;
                    }
                );
        }

        public static string GetScriptGuid(Type type)
        {
            return GetScriptGuids(type).SingleOrDefault();
        }

        public static IEnumerable<string> GetScriptGuids(Type type)
        {
            EnsureAnalyzed();

            using (var recursion = Recursion.New(1))
            {
                return GetScriptGuids(recursion, type).ToArray(); // No delayed execution for recursion disposal
            }
        }

        private static IEnumerable<string> GetScriptGuids(Recursion recursion, Type type)
        {
            if (!recursion?.TryEnter(type) ?? false)
            {
                yield break;
            }

            if (typesToGuids.ContainsKey(type))
            {
                foreach (var guid in typesToGuids[type])
                {
                    yield return guid;
                }
            }

            // Recurse inside the type.
            // For example, a List<Enemy> or an Enemy[] type should return the script GUID for Enemy.
            if (type.IsGenericType)
            {
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    foreach (var genericGuid in GetScriptGuids(recursion, genericArgument))
                    {
                        yield return genericGuid;
                    }
                }
            }
            else if (type.HasElementType)
            {
                foreach (var genericGuid in GetScriptGuids(recursion, type.GetElementType()))
                {
                    yield return genericGuid;
                }
            }

            recursion?.Exit(type);
        }

        public static IEnumerable<Type> GetScriptTypes(string guid)
        {
            EnsureAnalyzed();

            if (guidsToTypes.ContainsKey(guid))
            {
                return guidsToTypes[guid];
            }
            else
            {
                return Enumerable.Empty<Type>();
            }
        }

        public static IEnumerable<string> GetAllScriptGuids()
        {
            EnsureAnalyzed();

            return guids;
        }

        public static IEnumerable<string> GetModifiedScriptGuids(DateTime sinceUtc)
        {
            EnsureAnalyzed();

            foreach (var guidTimestamp in guidTimestamps)
            {
                if (guidTimestamp.Value > sinceUtc)
                {
                    yield return guidTimestamp.Key;
                }
            }
        }

        // The fileID for a loose script imported directly from a .cs file,
        // which is the class ID for MonoScript multiplied by 100,000.
        public const int CsFileID = 11500000;

        public static int GetFileID(Type type)
        {
            var guid = GetScriptGuid(type);

            var assetExtension = Path.GetExtension(AssetDatabase.GUIDToAssetPath(guid));

            switch (assetExtension.ToLowerInvariant())
            {
                case ".cs": return CsFileID;
                case ".dll": return GetDllFileID(type);
                default: throw new NotSupportedException($"Unknown type declarer asset extension: '{assetExtension}'.");
            }
        }

        public static int GetDllFileID(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            return GetDllFileID(type.Namespace, type.Name);
        }

        public static int GetDllFileID(string @namespace, string name)
        {
            // Gets the fileID of a given type inside a DLL asset.
            // "Given a type t, the fileID is equal to the first four bytes of the MD4 of the string `"s\0\0\0" + t.Namespace + t.Name' as a little endian 32-byte integer."
            // https://forum.unity.com/threads/yaml-fileid-hash-function-for-dll-scripts.252075/#post-1695479

            var toBeHashed = "s\0\0\0" + @namespace + name;

            using (var hash = new MD4())
            {
                var hashed = hash.ComputeHash(Encoding.UTF8.GetBytes(toBeHashed));

                var result = 0;

                for (var i = 3; i >= 0; --i)
                {
                    result <<= 8;
                    result |= hashed[i];
                }

                return result;
            }
        }
    }
}
