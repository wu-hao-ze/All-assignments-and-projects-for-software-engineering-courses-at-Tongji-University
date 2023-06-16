using System;
using System.Collections.Generic;

using UnityEditor;

using Codice.Client.Common.Threading;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Items;

namespace Unity.PlasticSCM.Editor.Views
{
    internal static class FileSystemOperation
    {
        internal static string GetExePath()
        {
            string title = PlasticLocalization.GetString(
                PlasticLocalization.Name.BrowseForExecutableFile);

            string directory = Environment.GetFolderPath(
                Environment.SpecialFolder.ProgramFiles);

            string path = EditorUtility.OpenFilePanel(title, directory, null);

            if (path.Length != 0)
                return path;

            return null;
        }

        internal static void Open(List<string> files)
        {
            try
            {
                foreach (string file in files)
                    OpenFile(file);
            }
            catch (Exception ex)
            {
                ExceptionsHandler.DisplayException(ex);
            }
        }

        internal static void OpenInExplorer(string path)
        {
            EditorUtility.RevealInFinder(path);
        }

        static void OpenFile(string path)
        {
            if (path == null)
                return;

            string relativePath = GetRelativePath.ToApplication(path);

            bool result = AssetDatabase.OpenAsset(
                AssetDatabase.LoadMainAssetAtPath(relativePath));

            if (result)
                return;

            OpenOperation.OpenFile(path);
        }
    }
}
