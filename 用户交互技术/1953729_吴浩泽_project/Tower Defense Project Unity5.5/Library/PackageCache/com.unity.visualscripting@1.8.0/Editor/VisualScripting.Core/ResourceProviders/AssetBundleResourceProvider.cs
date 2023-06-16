using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public sealed class AssetBundleResourceProvider : IResourceProvider
    {
        public const char DirectorySeparatorChar = '/';

        private AssetBundle _assetBundle;

        public AssetBundle assetBundle
        {
            get
            {
                if (_assetBundle == null)
                {
                    _assetBundle = AssetUtility.AssetBundleEditor;
                }

                return _assetBundle;
            }
        }

        public AssetBundleResourceProvider()
        {
            Analyze();
        }

        public AssetBundleResourceProvider(AssetBundle assetBundle)
        {
            if (_assetBundle != null)
            {
                _assetBundle.Unload(true);
            }
            _assetBundle = assetBundle;

            Analyze();
        }

        #region Filesystem

        public IEnumerable<string> GetAllFiles()
        {
            return assetBundle.GetAllAssetNames();
        }

        public IEnumerable<string> GetFiles(string path)
        {
            Ensure.That(nameof(path)).IsNotNull(path);

            path = NormalizePath(path);

            var directory = GetDirectory(path, true);

            foreach (var file in directory.files)
            {
                yield return $"{directory.path}{DirectorySeparatorChar}{file}";
            }
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            Ensure.That(nameof(path)).IsNotNull(path);

            path = NormalizePath(path);

            var directory = GetDirectory(path, true);

            foreach (var subDirectory in directory.subDirectories)
            {
                yield return subDirectory.Value.path;
            }
        }

        public string GetPersonalPath(string path, float width)
        {
            var name = Path.GetFileNameWithoutExtension(path).PartBefore('@');
            var extension = Path.GetExtension(path);
            var directory = Path.GetDirectoryName(path);

            return Path.Combine(directory, $"{name}@{width}x{extension}");
        }

        public string GetProfessionalPath(string path, float width)
        {
            var name = Path.GetFileNameWithoutExtension(path).PartBefore('@');
            var extension = Path.GetExtension(path);
            var directory = Path.GetDirectoryName(path);

            return Path.Combine(directory, $"{name}_Pro@{width}x{extension}");
        }

        public bool FileExists(string path)
        {
            Ensure.That(nameof(path)).IsNotNull(path);

            var directoryPath = NormalizePath(Path.GetDirectoryName(path));
            var fileName = NormalizePath(Path.GetFileName(path));

            var directory = GetDirectory(directoryPath, false);

            return directory != null && directory.files.Contains(fileName);
        }

        public bool DirectoryExists(string path)
        {
            Ensure.That(nameof(path)).IsNotNull(path);

            path = NormalizePath(path);

            return GetDirectory(path, false) != null;
        }

        public string NormalizePath(string path)
        {
            Ensure.That(nameof(path)).IsNotNull(path);

            return path.ToLower()
                .Replace(Path.DirectorySeparatorChar, DirectorySeparatorChar)
                .Replace(Path.AltDirectorySeparatorChar, DirectorySeparatorChar);
        }

        public string DebugPath(string path)
        {
            Ensure.That(nameof(path)).IsNotNull(path);

            path = NormalizePath(path);

            return $"{assetBundle.name}::{path}";
        }

        #endregion


        #region Loading

        public T LoadAsset<T>(string path) where T : UnityObject
        {
            Ensure.That(nameof(path)).IsNotNull(path);

            path = NormalizePath(path);

            return assetBundle.LoadAsset<T>(path);
        }

        public Texture2D LoadTexture(string path, CreateTextureOptions options)
        {
            return LoadAsset<Texture2D>(path);
        }

        #endregion


        #region Internals

        private void Analyze()
        {
            foreach (var path in assetBundle.GetAllAssetNames())
            {
                var directory = root;

                var parts = path.Split(DirectorySeparatorChar);

                for (var i = 0; i < parts.Length; i++)
                {
                    var isFile = i == parts.Length - 1;
                    var isDirectory = !isFile;
                    var part = parts[i];

                    if (isDirectory)
                    {
                        Directory subDirectory;

                        if (!directory.subDirectories.TryGetValue(part, out subDirectory))
                        {
                            subDirectory = new Directory(directory, part);
                            directory.subDirectories.Add(part, subDirectory);
                        }

                        directory = subDirectory;
                    }
                    else if (isFile)
                    {
                        directory.files.Add(part);
                    }
                }
            }
        }

        private readonly Directory root = new Directory(null, null);

        private Directory GetDirectory(string path, bool throwOnFail)
        {
            Ensure.That(nameof(path)).IsNotNull(path);

            path = NormalizePath(path);

            var parts = path.Split(DirectorySeparatorChar);

            var directory = root;

            foreach (var part in parts)
            {
                Directory subDirectory;

                if (!directory.subDirectories.TryGetValue(part, out subDirectory))
                {
                    if (throwOnFail)
                    {
                        throw new FileNotFoundException("Asset bundle directory not found.", DebugPath(path));
                    }

                    return null;
                }

                directory = subDirectory;
            }

            return directory;
        }

        private class Directory
        {
            public Directory parent { get; }

            public string name { get; }

            public string path { get; }

            public readonly Dictionary<string, Directory> subDirectories = new Dictionary<string, Directory>();

            public readonly List<string> files = new List<string>();

            public Directory(Directory parent, string name)
            {
                this.parent = parent;
                this.name = name;

                if (string.IsNullOrEmpty(parent?.path))
                {
                    path = name;
                }
                else
                {
                    path = $"{parent.path}{DirectorySeparatorChar}{name}";
                }
            }
        }

        #endregion
    }
}
