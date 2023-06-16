using System.IO;

using UnityEditor;

using Codice.Client.Common;
using PlasticGui;

namespace Unity.PlasticSCM.Editor.Views.History
{
    internal static class SaveAction
    {
        internal static string GetDestinationPath(
            string wkPath,
            string path,
            string defaultFileName)
        {
            string title = PlasticLocalization.GetString(
                PlasticLocalization.Name.SaveRevisionAs);

            string parentDirectory = GetDirectoryForSaveAs(wkPath, path);

            return EditorUtility.SaveFilePanel(
                title, parentDirectory, defaultFileName,
                string.Empty);
        }

        static string GetDirectoryForSaveAs(string wkPath, string path)
        {
            if (PathHelper.IsContainedOn(path, wkPath))
                return Path.GetDirectoryName(path);

            return WorkspacePath.GetWorkspacePathFromCmPath(
                wkPath,
                Path.GetDirectoryName(path),
                Path.DirectorySeparatorChar);
        }
    }
}
