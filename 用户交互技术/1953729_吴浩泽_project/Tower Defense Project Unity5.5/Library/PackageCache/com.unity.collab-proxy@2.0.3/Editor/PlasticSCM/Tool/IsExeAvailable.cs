using System;
using System.Collections.Generic;
using System.IO;

using Codice.Utils;

namespace Unity.PlasticSCM.Editor.Tool
{
    internal static class IsExeAvailable
    {
        internal static bool ForMode(bool isGluonMode)
        {
            string toolPath = isGluonMode ?
                PlasticInstallPath.GetGluonExePath() :
                PlasticInstallPath.GetPlasticExePath();

            return !string.IsNullOrEmpty(toolPath);
        }
    }

    internal static class PlasticInstallPath
    {
        internal static string GetClientBinDir()
        {
            if (PlatformIdentifier.IsWindows())
            {
                string plasticExePath = GetPlasticExePath();

                if (plasticExePath == null)
                    return null;

                return Path.GetDirectoryName(plasticExePath);
            }

            if (PlatformIdentifier.IsMac())
            {
                string path = GetToolCommand(Plastic.NEW_GUI_MACOS);
                if (path != null)
                    return GetExistingDir(ToolConstants.NEW_MACOS_BINDIR);
               
                return GetExistingDir(ToolConstants.LEGACY_MACOS_BINDIR);
            }

            return null;
        }

        internal static string GetPlasticExePath()
        {
            if (PlatformIdentifier.IsWindows())
                return FindTool.ObtainToolCommand(
                Plastic.GUI_WINDOWS,
                    new List<String>() { GetWindowsInstallationFolder() });

            if (PlatformIdentifier.IsMac())
            {
                string path = GetToolCommand(Plastic.NEW_GUI_MACOS);
                if(path != null)
                    return path;

                return GetToolCommand(Plastic.LEGACY_GUI_MACOS);
            }

            return null;
        }

        internal static string GetGluonExePath()
        {
            if (PlatformIdentifier.IsWindows())
                return FindTool.ObtainToolCommand(
                    Gluon.GUI_WINDOWS,
                    new List<String>() { GetWindowsInstallationFolder() });

            if (PlatformIdentifier.IsMac())
            {
                string path = GetToolCommand(Gluon.NEW_GUI_MACOS);
                if (path != null)
                    return path;

                return GetToolCommand(Gluon.LEGACY_GUI_MACOS);
            }

            return null;
        }

        static string GetToolCommand(string tool)
        {
            return File.Exists(tool) ? tool : null;
        }

        static string GetExistingDir(string directory)
        {
            return Directory.Exists(directory) ? directory : null;
        }

        static string GetWindowsInstallationFolder()
        {
            string programFilesFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.ProgramFiles);

            return Path.Combine(Path.Combine(programFilesFolder,
                PLASTICSCM_FOLDER), PLASTICSCM_SUBFOLDER);
        }

        const string PLASTICSCM_FOLDER = "PlasticSCM5";
        const string PLASTICSCM_SUBFOLDER = "client";

        class Plastic
        {
            internal const string GUI_WINDOWS = "plastic.exe";
            internal const string LEGACY_GUI_MACOS = "/Applications/PlasticSCM.app/Contents/MacOS/PlasticSCM";
            internal const string NEW_GUI_MACOS = "/Applications/PlasticSCM.app/Contents/MacOS/macplasticx";
        }

        class Gluon
        {
            internal const string GUI_WINDOWS = "gluon.exe";
            internal const string LEGACY_GUI_MACOS = "/Applications/Gluon.app/Contents/MacOS/Gluon";
            internal const string NEW_GUI_MACOS = "/Applications/Gluon.app/Contents/MacOS/macgluonx";
        }
    }
}