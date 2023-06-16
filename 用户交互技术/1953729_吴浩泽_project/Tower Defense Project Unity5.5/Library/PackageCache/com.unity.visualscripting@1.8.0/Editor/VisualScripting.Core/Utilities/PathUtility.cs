using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Unity.VisualScripting
{
    public static class PathUtility
    {
        public static string TryPathsForFile(string fileName, IEnumerable<string> directories)
        {
            return directories.Select(directory => Path.Combine(directory, fileName)).FirstOrDefault(File.Exists);
        }

        public static string TryPathsForFile(string fileName, params string[] directories)
        {
            return TryPathsForFile(fileName, (IEnumerable<string>)directories);
        }

        public static string GetRelativePath(string path, string directory)
        {
            Ensure.That(nameof(path)).IsNotNull(path);
            Ensure.That(nameof(directory)).IsNotNull(directory);

            if (!directory.EndsWith(Path.DirectorySeparatorChar))
            {
                directory += Path.DirectorySeparatorChar;
            }

            try
            {
                // Optimization: Try a simple substring if possible

                path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                directory = directory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                if (path.StartsWith(directory, StringComparison.Ordinal))
                {
                    return path.Substring(directory.Length);
                }

                // Otherwise, use the URI library

                var pathUri = new Uri(path);
                var folderUri = new Uri(directory);

                return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString()
                    .Replace('/', Path.DirectorySeparatorChar));
            }
            catch (UriFormatException ufex)
            {
                throw new UriFormatException($"Failed to get relative path.\nPath: {path}\nDirectory:{directory}\n{ufex}");
            }
        }

        public static string FromEditorResources(string path)
        {
            return GetRelativePath(path, Paths.editorDefaultResources);
        }

        public static string FromAssets(string path)
        {
            return GetRelativePath(path, Paths.assets);
        }

        public static string FromProject(string path)
        {
            return GetRelativePath(path, Paths.project);
        }

        public static void CreateParentDirectoryIfNeeded(string path)
        {
            CreateDirectoryIfNeeded(Directory.GetParent(path).FullName);
        }

        public static void CreateDirectoryIfNeeded(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void DeleteDirectoryIfExists(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            var metaFilePath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path) + ".meta");

            if (File.Exists(metaFilePath))
            {
                File.Delete(metaFilePath);
            }
        }

        public static void DeleteProjectFileIfExists(string filePath, bool checkoutInVersionControl = false)
        {
            if (File.Exists(filePath))
            {
                string metaFilePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileName(filePath) + ".meta");

                if (checkoutInVersionControl)
                {
                    VersionControlUtility.Unlock(filePath);
                    VersionControlUtility.Unlock(metaFilePath);
                }
                File.Delete(filePath);
                File.Delete(metaFilePath);
            }
        }

        public static string MakeSafeFilename(string filename, char replace)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, replace);
            }

            return filename;
        }

        public static string GetPackageRootPath()
        {
            // This bit of code is so that we can find out Bolt package folder wherever it lives, as long as its accessible to the AssetDatabase
            const string packageEditorFolderGuid = "ee9f54d6cd5a17045a3f1c9877d4b7e6";
            var packageEditorAssetPath = AssetDatabase.GUIDToAssetPath(packageEditorFolderGuid);
            if (packageEditorAssetPath == string.Empty)
            {
                throw new FileNotFoundException($"Couldn't find Visual Scripting package folder.");
            }

            // The root VS folder path is 1 directories up from our Editor folder
            return Path.GetDirectoryName(packageEditorAssetPath);
        }
    }
}
