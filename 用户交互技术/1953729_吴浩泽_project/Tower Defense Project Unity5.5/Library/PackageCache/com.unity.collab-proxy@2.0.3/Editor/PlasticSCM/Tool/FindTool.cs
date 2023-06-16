using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Unity.PlasticSCM.Editor.Tool
{
    internal static class FindTool
    {
        internal static string ObtainToolCommand(
            string toolName, List<string> installationPaths)
        {
            List<string> processPaths = GetPathsFromEnvVariable(
                PATH_ENVIRONMENT_VARIABLE,
                EnvironmentVariableTarget.Process);

            List<string> machinePaths = GetPathsFromEnvVariable(
                PATH_ENVIRONMENT_VARIABLE,
                EnvironmentVariableTarget.Machine);

            List<string> pathsToLookup = new List<string>();
            pathsToLookup.AddRange(processPaths);
            pathsToLookup.AddRange(machinePaths);
            pathsToLookup.AddRange(installationPaths);

            string toolPath = FindToolInPaths(toolName, pathsToLookup);

            if (string.IsNullOrEmpty(toolPath))
                return null;

            EnsureIsInProcessPathEnvVariable(toolPath, processPaths);

            return toolPath;
        }

        static string FindToolInPaths(
            string toolName,
            List<string> paths)
        {
            foreach (string path in paths)
            {
                if (path == null)
                    continue;

                if (path.Trim() == string.Empty)
                    continue;

                string filePath = CleanFolderPath(path);

                filePath = Path.Combine(filePath, toolName);

                if (File.Exists(filePath))
                    return Path.GetFullPath(filePath);
            }

            return null;
        }

        static string CleanFolderPath(string folderPath)
        {
            foreach (char c in Path.GetInvalidPathChars())
                folderPath = folderPath.Replace(c.ToString(), string.Empty);

            return folderPath;
        }

        static List<string> GetPathsFromEnvVariable(
            string variableName,
            EnvironmentVariableTarget target)
        {
            string value = Environment.GetEnvironmentVariable(variableName, target);
            return new List<string>(value.Split(Path.PathSeparator));
        }

        static void EnsureIsInProcessPathEnvVariable(
            string toolPath,
            List<string> processPaths)
        {
            string plasticInstallDir = Path.GetDirectoryName(toolPath);

            if (processPaths.Contains(plasticInstallDir, StringComparer.OrdinalIgnoreCase))
                return;

            Environment.SetEnvironmentVariable(
                PATH_ENVIRONMENT_VARIABLE,
                string.Concat(plasticInstallDir, Path.PathSeparator, processPaths),
                EnvironmentVariableTarget.Process);
        }

        const string PATH_ENVIRONMENT_VARIABLE = "PATH";
    }
}
