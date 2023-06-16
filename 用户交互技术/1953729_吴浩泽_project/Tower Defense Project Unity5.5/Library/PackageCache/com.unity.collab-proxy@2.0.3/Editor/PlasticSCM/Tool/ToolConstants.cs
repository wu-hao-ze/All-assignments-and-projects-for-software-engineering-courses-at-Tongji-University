namespace Unity.PlasticSCM.Editor.Tool
{
    internal static class ToolConstants
    {
        internal static class Plastic
        {

            internal const string GUI_CONFIGURE_ARG = "--configure";

            internal const string GUI_WINDOWS_WK_ARG = "--wk=\"{0}\"";
            internal const string GUI_WINDOWS_BREX_ARG = "--branchexplorer=\"{0}\"";
            internal const string GUI_WINDOWS_MERGE_ARG = "--resolve=\"{0}\"";
            internal const string GUI_WINDOWS_INCOMING_CHANGES_ARG = "--resolve=\"{0}\" --incomingmerge";

            internal const string GUI_MACOS_WK_EXPLORER_ARG = "--wk=\"{0}\" --view=ItemsView";
            internal const string GUI_MACOS_BREX_ARG = "--wk=\"{0}\" --view=BranchExplorerView";
            internal const string GUI_MACOS_MERGE_ARG = "--wk=\"{0}\" --view=MergeView";
            internal const string GUI_MACOS_INCOMING_CHANGES_ARG = "--wk=\"{0}\" --view=IncomingChangesView";
            internal const string GUI_MACOS_COMMAND_FILE_ARG = " --command-file=\"{0}\"";
            internal const string GUI_MACOS_COMMAND_FILE = "macplastic-command-file.txt";

            internal const string GUI_CHANGESET_DIFF_ARG = "--diffchangeset=\"{0}\"";
            internal const string GUI_SELECTED_CHANGESETS_DIFF_ARGS = "--diffchangesetsrc=\"{0}\" --diffchangesetdst=\"{1}\"";
            internal const string GUI_BRANCH_DIFF_ARG = "--diffbranch=\"{0}\"";
        }

        internal static class Gluon
        {

            internal const string GUI_CONFIGURE_ARG = "--configure";

            internal const string GUI_WK_EXPLORER_ARG = "--wk=\"{0}\" --view=WorkspaceExplorerView";
            internal const string GUI_WK_CONFIGURATION_ARG = "--wk=\"{0}\" --view=WorkspaceConfigurationView";
            internal const string GUI_WK_INCOMING_CHANGES_ARG = "--wk=\"{0}\" --view=IncomingChangesView";
            internal const string GUI_COMMAND_FILE_ARG = " --command-file=\"{0}\"";
            internal const string GUI_COMMAND_FILE = "gluon-command-file.txt";

            internal const string GUI_CHANGESET_DIFF_ARG = "--diffchangeset=\"{0}\"";
            internal const string GUI_SELECTED_CHANGESETS_DIFF_ARGS = "--diffchangesetsrc=\"{0}\" --diffchangesetdst=\"{1}\"";
            internal const string GUI_BRANCH_DIFF_ARG = "--diffbranch=\"{0}\"";
        }

        internal static class Installer
        {
            internal const string INSTALLER_WINDOWS_ARGS = "--mode unattended --unattendedmodeui minimal";
            internal const string INSTALLER_MACOS_OPEN = "open";
            internal const string INSTALLER_MACOS_OPEN_ARGS = "-W -n {0}";
        }

        internal const string LEGACY_MACOS_BINDIR = "/Applications/PlasticSCM.app/Contents/MonoBundle";
        internal const string NEW_MACOS_BINDIR = "/Applications/PlasticSCM.app/Contents/MacOS";
    }
}
