using System;
using System.Diagnostics;
using System.IO;

using Codice.Client.BaseCommands.EventTracking;
using Codice.CM.Common;
using Codice.LogWrapper;
using Codice.Utils;
using Unity.PlasticSCM.Editor.Views;

namespace Unity.PlasticSCM.Editor.Tool
{
    internal static class LaunchTool
    {
        public interface IShowDownloadPlasticExeWindow
        {
            bool Show(
                WorkspaceInfo wkInfo,
                bool isGluonMode,
                string installCloudFrom,
                string installEnterpriseFrom,
                string cancelInstallFrom);
            bool Show(
                RepositorySpec repSpec,
                bool isGluonMode,
                string installCloudFrom,
                string installEnterpriseFrom,
                string cancelInstallFrom);
        }

        public interface IProcessExecutor
        {
            Process ExecuteGUI(
                string program,
                string args,
                string commandFileArg,
                string commandFileName,
                int processId);
            Process ExecuteWindowsGUI(
                string program,
                string args,
                int processId);
            Process ExecuteProcess(string program, string args);
        }

        internal static void OpenGUIForMode(
            IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            IProcessExecutor processExecutor,
            WorkspaceInfo wkInfo,
            bool isGluonMode)
        {
            if (showDownloadPlasticExeWindow.Show(
                wkInfo,
                isGluonMode,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromOpenGUI,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromOpenGUI,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromOpenGUI))
                return;

            mLog.DebugFormat(
                "Opening GUI on wkPath '{0}'.",
                wkInfo.ClientPath);

            TrackFeatureUseEvent.For(
                PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
                isGluonMode ?
                    TrackFeatureUseEvent.Features.LaunchGluonTool :
                    TrackFeatureUseEvent.Features.LaunchPlasticTool);

            if (isGluonMode)
            {
                Process gluonProcess = processExecutor.ExecuteGUI(
                    PlasticInstallPath.GetGluonExePath(),
                    string.Format(
                        ToolConstants.Gluon.GUI_WK_EXPLORER_ARG,
                        wkInfo.ClientPath),
                    ToolConstants.Gluon.GUI_COMMAND_FILE_ARG,
                    ToolConstants.Gluon.GUI_COMMAND_FILE,
                    mGluonProcessId);

                if (gluonProcess != null)
                    mGluonProcessId = gluonProcess.Id;

                return;
            }

            if (PlatformIdentifier.IsMac())
            {
                Process plasticProcess = processExecutor.ExecuteGUI(
                    PlasticInstallPath.GetPlasticExePath(),
                    string.Format(
                        ToolConstants.Plastic.GUI_MACOS_WK_EXPLORER_ARG,
                        wkInfo.ClientPath),
                    ToolConstants.Plastic.GUI_MACOS_COMMAND_FILE_ARG,
                    ToolConstants.Plastic.GUI_MACOS_COMMAND_FILE,
                    mPlasticProcessId);

                if (plasticProcess != null)
                    mPlasticProcessId = plasticProcess.Id;

                return;
            }

            processExecutor.ExecuteProcess(
                PlasticInstallPath.GetPlasticExePath(),
                string.Format(
                    ToolConstants.Plastic.GUI_WINDOWS_WK_ARG,
                    wkInfo.ClientPath));
        }

        internal static void OpenBranchExplorer(
            IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            IProcessExecutor processExecutor,
            WorkspaceInfo wkInfo,
            bool isGluonMode)
        {
            if (showDownloadPlasticExeWindow.Show(
                wkInfo,
                isGluonMode,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromOpenBranchExplorer,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromOpenBranchExplorer,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromOpenBranchExplorer))
                return;

            mLog.DebugFormat(
                "Opening Branch Explorer on wkPath '{0}'.",
                wkInfo.ClientPath);

            TrackFeatureUseEvent.For(
                PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
                TrackFeatureUseEvent.Features.LaunchBranchExplorer);

            if (PlatformIdentifier.IsMac())
            {
                Process plasticProcess = processExecutor.ExecuteGUI(
                    PlasticInstallPath.GetPlasticExePath(),
                    string.Format(
                        ToolConstants.Plastic.GUI_MACOS_BREX_ARG,
                        wkInfo.ClientPath),
                    ToolConstants.Plastic.GUI_MACOS_COMMAND_FILE_ARG,
                    ToolConstants.Plastic.GUI_MACOS_COMMAND_FILE,
                    mPlasticProcessId);

                if (plasticProcess != null)
                    mPlasticProcessId = plasticProcess.Id;

                return;
            }

            Process brexProcess = processExecutor.ExecuteWindowsGUI(
                PlasticInstallPath.GetPlasticExePath(),
                string.Format(
                    ToolConstants.Plastic.GUI_WINDOWS_BREX_ARG,
                    wkInfo.ClientPath),
                mBrexProcessId);

            if (brexProcess != null)
                mBrexProcessId = brexProcess.Id;
        }

        internal static void OpenChangesetDiffs(
            IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            IProcessExecutor processExecutor,
            RepositorySpec repSpec,
            string fullChangesetSpec,
            bool isGluonMode)
        {
            if (showDownloadPlasticExeWindow.Show(
                repSpec,
                isGluonMode,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromOpenChangesetDiffs,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromOpenChangesetDiffs,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromOpenChangesetDiffs))
                return;

            mLog.DebugFormat(
                "Launching changeset diffs for '{0}'",
                fullChangesetSpec);

            string exePath = (isGluonMode) ?
                PlasticInstallPath.GetGluonExePath() :
                PlasticInstallPath.GetPlasticExePath();

            string changesetDiffArg = (isGluonMode) ?
                ToolConstants.Gluon.GUI_CHANGESET_DIFF_ARG :
                ToolConstants.Plastic.GUI_CHANGESET_DIFF_ARG;

            processExecutor.ExecuteProcess(exePath,
                string.Format(
                    changesetDiffArg, fullChangesetSpec));
        }

        internal static void OpenSelectedChangesetsDiffs(
            IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            IProcessExecutor processExecutor,
            RepositorySpec repSpec,
            string srcFullChangesetSpec,
            string dstFullChangesetSpec,
            bool isGluonMode)
        {
            if (showDownloadPlasticExeWindow.Show(
                repSpec,
                isGluonMode,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromOpenSelectedChangesetsDiffs,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromOpenSelectedChangesetsDiffs,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromOpenSelectedChangesetsDiffs))
                return;

            mLog.DebugFormat(
                "Launching selected changesets diffs for '{0}' and '{1}'",
                srcFullChangesetSpec,
                dstFullChangesetSpec);

            string exePath = (isGluonMode) ?
                PlasticInstallPath.GetGluonExePath() :
                PlasticInstallPath.GetPlasticExePath();

            string selectedChangesetsDiffArgs = (isGluonMode) ?
                ToolConstants.Gluon.GUI_SELECTED_CHANGESETS_DIFF_ARGS :
                ToolConstants.Plastic.GUI_SELECTED_CHANGESETS_DIFF_ARGS;

            processExecutor.ExecuteProcess(exePath,
                string.Format(
                    selectedChangesetsDiffArgs,
                    srcFullChangesetSpec,
                    dstFullChangesetSpec));
        }

        internal static void OpenBranchDiffs(
            IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            IProcessExecutor processExecutor,
            RepositorySpec repSpec,
            string fullBranchSpec,
            bool isGluonMode)
        {
            if (showDownloadPlasticExeWindow.Show(
                repSpec,
                isGluonMode,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromOpenBranchDiff,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromOpenBranchDiff,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromOpenBranchDiff))
                return;

            mLog.DebugFormat(
                "Launching branch diffs for '{0}'",
                fullBranchSpec);

            string exePath = (isGluonMode) ?
                PlasticInstallPath.GetGluonExePath() :
                PlasticInstallPath.GetPlasticExePath();

            string branchDiffArg = (isGluonMode) ?
                ToolConstants.Gluon.GUI_BRANCH_DIFF_ARG :
                ToolConstants.Plastic.GUI_BRANCH_DIFF_ARG;

            processExecutor.ExecuteProcess(exePath,
                string.Format(
                    branchDiffArg, fullBranchSpec));
        }

        internal static void OpenWorkspaceConfiguration(
            IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            IProcessExecutor processExecutor,
            WorkspaceInfo wkInfo,
            bool isGluonMode)
        {
            if (showDownloadPlasticExeWindow.Show(
                wkInfo,
                isGluonMode,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromOpenWorkspaceConfiguration,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromOpenWorkspaceConfiguration,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromOpenWorkspaceConfiguration))
                return;

            mLog.DebugFormat(
                "Opening Workspace Configuration on wkPath '{0}'.",
                wkInfo.ClientPath);

            TrackFeatureUseEvent.For(
                PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
                TrackFeatureUseEvent.Features.LaunchPartialConfigure);

            Process gluonProcess = processExecutor.ExecuteGUI(
                PlasticInstallPath.GetGluonExePath(),
                string.Format(
                    ToolConstants.Gluon.GUI_WK_CONFIGURATION_ARG,
                    wkInfo.ClientPath),
                ToolConstants.Gluon.GUI_COMMAND_FILE_ARG,
                ToolConstants.Gluon.GUI_COMMAND_FILE,
                mGluonProcessId);

            if (gluonProcess == null)
                return;

            mGluonProcessId = gluonProcess.Id;
        }

        internal static void OpenMerge(
            IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            IProcessExecutor processExecutor,
            WorkspaceInfo wkInfo,
            bool isGluonMode)
        {
            if (showDownloadPlasticExeWindow.Show(
                wkInfo,
                isGluonMode,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromOpenMerge,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromOpenMerge,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromOpenMerge))
                return;

            mLog.DebugFormat(
                "Opening Merge on wkPath '{0}'.",
                wkInfo.ClientPath);

            if (PlatformIdentifier.IsMac())
            {
                Process plasticProcess = processExecutor.ExecuteGUI(
                    PlasticInstallPath.GetPlasticExePath(),
                    string.Format(ToolConstants.Plastic.GUI_MACOS_MERGE_ARG, wkInfo.ClientPath),
                    ToolConstants.Plastic.GUI_MACOS_COMMAND_FILE_ARG,
                    ToolConstants.Plastic.GUI_MACOS_COMMAND_FILE,
                    mPlasticProcessId);

                if (plasticProcess != null)
                    mPlasticProcessId = plasticProcess.Id;

                return;
            }

            processExecutor.ExecuteProcess(
                PlasticInstallPath.GetPlasticExePath(),
                string.Format(ToolConstants.Plastic.GUI_WINDOWS_MERGE_ARG, wkInfo.ClientPath));
        }

        static int mPlasticProcessId = -1;
        static int mGluonProcessId = -1;
        static int mBrexProcessId = -1;

        static readonly ILog mLog = LogManager.GetLogger("LaunchTool");

        internal class ShowDownloadPlasticExeWindow : LaunchTool.IShowDownloadPlasticExeWindow
        {
            bool LaunchTool.IShowDownloadPlasticExeWindow.Show(
                WorkspaceInfo wkInfo,
                bool isGluonMode,
                string installCloudFrom,
                string installEnterpriseFrom,
                string cancelInstallFrom)
            {
                RepositorySpec repSpec = PlasticGui.Plastic.API.GetRepositorySpec(wkInfo);

                return ((LaunchTool.IShowDownloadPlasticExeWindow)this).Show(
                    repSpec,
                    isGluonMode,
                    installCloudFrom,
                    installEnterpriseFrom,
                    cancelInstallFrom);
            }

            bool LaunchTool.IShowDownloadPlasticExeWindow.Show(
                RepositorySpec repSpec,
                bool isGluonMode,
                string installCloudFrom,
                string installEnterpriseFrom,
                string cancelInstallFrom)
            {
                if (IsExeAvailable.ForMode(isGluonMode))
                    return false;

                DownloadPlasticExeWindow.ShowWindow(
                    repSpec,
                    isGluonMode,
                    installCloudFrom,
                    installEnterpriseFrom,
                    cancelInstallFrom);

                return true;
            }
        }

        internal class ProcessExecutor : LaunchTool.IProcessExecutor
        {
            Process LaunchTool.IProcessExecutor.ExecuteGUI(
                string program,
                string args,
                string commandFileArg,
                string commandFileName,
                int processId)
            {
                string commandFile = Path.Combine(
                    Path.GetTempPath(), commandFileName);

                Process process = GetGUIProcess(program, processId);

                if (process == null)
                {
                    mLog.DebugFormat("Executing {0} (new process).", program);

                    return ((LaunchTool.IProcessExecutor)this).ExecuteProcess(
                        program, args + string.Format(commandFileArg, commandFile));
                }

                mLog.DebugFormat("Executing {0} (reuse process pid:{1}).", program, processId);

                using (StreamWriter writer = new StreamWriter(new FileStream(
                    commandFile, FileMode.Append, FileAccess.Write, FileShare.Read)))
                {
                    writer.WriteLine(args);
                }

                return process;
            }

            Process LaunchTool.IProcessExecutor.ExecuteWindowsGUI(
                string program,
                string args,
                int processId)
            {
                Process process = GetGUIProcess(program, processId);

                if (process == null)
                {
                    mLog.DebugFormat("Executing {0} (new process).", program);

                    return ((LaunchTool.IProcessExecutor)this).ExecuteProcess(program, args);
                }

                mLog.DebugFormat("Not executing {0} (existing process pid:{1}).", program, processId);

                BringWindowToFront.ForWindowsProcess(process.Id);

                return process;
            }

            Process LaunchTool.IProcessExecutor.ExecuteProcess(string program, string args)
            {
                mLog.DebugFormat("Execute process: '{0} {1}'", program, args);

                Process process = BuildProcess(program, args);

                try
                {
                    process.Start();

                    return process;
                }
                catch (Exception ex)
                {
                    mLog.ErrorFormat("Couldn't execute the program {0}: {1}",
                        program, ex.Message);
                    mLog.DebugFormat("Stack trace: {0}",
                        ex.StackTrace);

                    return null;
                }
            }

            Process BuildProcess(string program, string args)
            {
                Process result = new Process();
                result.StartInfo.FileName = program;
                result.StartInfo.Arguments = args;
                result.StartInfo.CreateNoWindow = false;
                return result;
            }

            Process GetGUIProcess(string program, int processId)
            {
                if (processId == -1)
                    return null;

                mLog.DebugFormat("Checking {0} process [pid:{1}].", program, processId);

                try
                {
                    Process process = Process.GetProcessById(processId);

                    if (process == null)
                        return null;

                    return process.HasExited ? null : process;
                }
                catch
                {
                    // process is not running
                    return null;
                }
            }

            readonly ILog mLog = LogManager.GetLogger("ProcessExecutor");
        }
    }
}
