using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Unity.VisualScripting
{
    public static class Paths
    {
        static Paths()
        {
            assets = Application.dataPath;
            editor = EditorApplication.applicationPath;
            editorContents = EditorApplication.applicationContentsPath;

            try
            {
                SyncVS = typeof(Editor).Assembly.GetType("UnityEditor.SyncVS", true);
                SyncVS_SyncSolution = SyncVS.GetMethod("SyncSolution", BindingFlags.Static | BindingFlags.Public);

                if (SyncVS_SyncSolution == null)
                {
                    throw new MissingMemberException(SyncVS.ToString(), "SyncSolution");
                }
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
        }

        public static string assets { get; }

        public static string editor { get; }

        public static string editorContents { get; }

        public static string project => Directory.GetParent(assets).FullName;

        public static string projectName => Path.GetFileName(project.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        public static string projectSettings => Path.Combine(project, "ProjectSettings");

        public static string editorDefaultResources => Path.Combine(assets, "Editor Default Resources");

        public static string backups => Path.Combine(project, "Backups");


        #region Assembly Projects

        private static Type SyncVS; // internal class UnityEditor.SyncVS : AssetPostprocessor

        private static MethodInfo SyncVS_SyncSolution; // public static void SyncSolution()

        public static void SyncUnitySolution()
        {
            try
            {
                SyncVS_SyncSolution.Invoke(null, null);
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
        }

        public static string runtimeAssemblyFirstPassProject =>
            PreferredProjectPath
            (
                Path.Combine(project, projectName + ".Plugins.csproj"),
                Path.Combine(project, "Assembly-CSharp-firstpass.csproj")
            );

        public static string runtimeAssemblySecondPassProject =>
            PreferredProjectPath
            (
                Path.Combine(project, projectName + ".csproj"),
                Path.Combine(project, "Assembly-CSharp.csproj")
            );

        public static string editorAssemblyFirstPassProject =>
            PreferredProjectPath
            (
                Path.Combine(project, projectName + ".Editor.csproj"),
                Path.Combine(project, "Assembly-CSharp-Editor-firstpass.csproj")
            );

        public static string editorAssemblySecondPassProject =>
            PreferredProjectPath
            (
                Path.Combine(project, projectName + ".Editor.Plugins.csproj"),
                Path.Combine(project, "Assembly-CSharp-Editor.csproj")
            );

        public static IEnumerable<string> assemblyProjects
        {
            get
            {
                var firstPass = runtimeAssemblyFirstPassProject;
                var secondPass = runtimeAssemblySecondPassProject;
                var editorFirstPass = editorAssemblyFirstPassProject;
                var editorSecondPass = editorAssemblySecondPassProject;

                if (firstPass != null)
                {
                    yield return firstPass;
                }

                if (secondPass != null)
                {
                    yield return secondPass;
                }

                if (editorFirstPass != null)
                {
                    yield return editorFirstPass;
                }

                if (editorSecondPass != null)
                {
                    yield return editorSecondPass;
                }
            }
        }

        private static string PreferredProjectPath(string path1, string path2)
        {
            if (!File.Exists(path1) && !File.Exists(path2))
            {
                return null;
            }

            if (!File.Exists(path1))
            {
                return path2;
            }

            if (!File.Exists(path2))
            {
                return path1;
            }

            var timestamp1 = File.GetLastWriteTime(path1);
            var timestamp2 = File.GetLastWriteTime(path2);

            if (timestamp1 >= timestamp2)
            {
                return path1;
            }

            return path2;
        }

        #endregion


        #region .NET

        public const string MsBuildDownloadLink = "https://aka.ms/vs/15/release/vs_buildtools.exe";

        private static IEnumerable<string> environmentPaths
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    return Environment.GetEnvironmentVariable("PATH").Split(';');
                }

                // http://stackoverflow.com/a/41318134/154502
                var start = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-l -c \"echo $PATH\"", // -l = 'login shell' to execute /etc/profile
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = Process.Start(start);
                process.WaitForExit();
                var path = process.StandardOutput.ReadToEnd().Trim();
                return path.Split(':');
            }
        }

        // ProgramFilesx86 is not available until .NET 4
        // https://stackoverflow.com/questions/194157/
        private static string ProgramFilesx86
        {
            get
            {
                if (IntPtr.Size == 8 || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))
                {
                    return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                }

                return Environment.GetEnvironmentVariable("ProgramFiles");
            }
        }

        public static string msBuild
        {
            get
            {
                if (Application.platform != RuntimePlatform.WindowsEditor)
                {
                    return null;
                }

                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine(ProgramFilesx86, @"Microsoft Visual Studio\Installer\vswhere.exe"),
                        Arguments = @"-latest -prerelease -products * -requires Microsoft.Component.MSBuild -find **\Bin\MSBuild.exe",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    using (var vsWhere = Process.Start(startInfo))
                    {
                        var firstPath = vsWhere.StandardOutput.ReadLine();
                        vsWhere.WaitForExit();
                        return firstPath;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to find MSBuild path via VSWhere utility.\n{ex}");
                    return null;
                }
            }
        }

        public static string xBuild
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    return null;
                }

                var path = PathUtility.TryPathsForFile("xbuild", environmentPaths);

                return path;
            }
        }

        public static string roslynCompiler => Path.Combine(Path.GetDirectoryName(editor), "Data/tools/Roslyn/csc.exe");

        public static string projectBuilder => Application.platform == RuntimePlatform.WindowsEditor ? msBuild : xBuild;

        #endregion
    }
}
