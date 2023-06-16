namespace Unity.PlasticSCM.Editor.UI
{
    public static class UnityConstants
    {
        internal const float CANCEL_BUTTON_SIZE = 15f;

        internal const float SMALL_BUTTON_WIDTH = 40f;
        internal const float REGULAR_BUTTON_WIDTH = 60f;
        internal const float LARGE_BUTTON_WIDTH = 100f;
        internal const float EXTRA_LARGE_BUTTON_WIDTH = 130f;

        internal const float SEARCH_FIELD_WIDTH = 550f;
        internal const float DIFF_PANEL_MIN_WIDTH = SEARCH_FIELD_WIDTH / 2f + 8f;

        internal const string TREEVIEW_META_LABEL = " +meta";
        internal const float TREEVIEW_CHECKBOX_SIZE = 17f;
        internal const float TREEVIEW_BASE_INDENT = 16f;
        internal const float TREEVIEW_ROW_WIDTH_OFFSET = 24f;
        internal const int TREEVIEW_STATUS_ICON_SIZE = 16;
        internal const int TREEVIEW_STATUS_CONTENT_PADDING = 4;
        internal const int TREEVIEW_STATUS_CONTENT_OFFSET = 2;
        internal const float FIRST_COLUMN_WITHOUT_ICON_INDENT = 5f;
        internal const int PENDING_CHANGES_FONT_SIZE = 12;
        internal const int OVERLAY_STATUS_ICON_SIZE = 16;

        internal const int SETTINGS_GUI_WIDTH = 400;

        internal const int STATUS_BAR_HEIGHT = 24;
        internal const int STATUS_BAR_ICON_SIZE = 16;
        internal const int STATUS_BAR_NOTIFICATION_SPACING = 8;

#if UNITY_2019_1_OR_NEWER
        internal const float DROPDOWN_ICON_Y_OFFSET = 2f;
        internal const float TREEVIEW_FOLDOUT_Y_OFFSET = 0f;
        internal const float TREEVIEW_ROW_HEIGHT = 24f;
        internal const float TREEVIEW_HEADER_CHECKBOX_Y_OFFSET = 0f;
        internal const float TREEVIEW_CHECKBOX_Y_OFFSET = 0f;
        internal static float DIR_CONFLICT_VALIDATION_WARNING_LABEL_HEIGHT = 21f;
#else
        internal const float DROPDOWN_ICON_Y_OFFSET = 5f;
        internal const float TREEVIEW_FOLDOUT_Y_OFFSET = 1f;
        internal const float TREEVIEW_ROW_HEIGHT = 20f;
        internal const float TREEVIEW_PENDING_CHANGES_ROW_HEIGHT = 24f;
        internal const float TREEVIEW_HEADER_CHECKBOX_Y_OFFSET = 6f;
        internal const float TREEVIEW_CHECKBOX_Y_OFFSET = 2f;
        internal static float DIR_CONFLICT_VALIDATION_WARNING_LABEL_HEIGHT = 18f;
#endif

#if UNITY_2020_1_OR_NEWER
        internal const float INSPECTOR_ACTIONS_BACK_RECTANGLE_TOP_MARGIN = -2f;
#else
        internal const float INSPECTOR_ACTIONS_BACK_RECTANGLE_TOP_MARGIN = 0f;
#endif

#if UNITY_2019
        internal const int INSPECTOR_ACTIONS_HEADER_BACK_RECTANGLE_HEIGHT = 24;
#else
        internal const int INSPECTOR_ACTIONS_HEADER_BACK_RECTANGLE_HEIGHT = 7;
#endif


        internal const int LEFT_MOUSE_BUTTON = 0;
        internal const int RIGHT_MOUSE_BUTTON = 1;

        internal const int UNSORT_COLUMN_ID = -1;

        internal const string PLASTIC_WINDOW_TITLE = "Unity Version Control";
        internal const string PROJECT_SETTINGS_TAB_PATH = "Project/Version Control/Unity Version Control";
        internal const string PROJECT_SETTINGS_TAB_TITLE = "Unity Version Control Settings";
        internal const float PLASTIC_WINDOW_MIN_SIZE_WIDTH = 600f;
        internal const float PLASTIC_WINDOW_MIN_SIZE_HEIGHT = 350f;
        internal const float PLASTIC_WINDOW_COMMENT_SECTION_HEIGHT = 55f;

        internal const int ACTIVE_TAB_UNDERLINE_HEIGHT = 1;
        internal const int SPLITTER_INDICATOR_HEIGHT = 1;

        internal const double SEARCH_DELAYED_INPUT_ACTION_INTERVAL = 0.25;
        internal const double SELECTION_DELAYED_INPUT_ACTION_INTERVAL = 0.25;
        internal const double AUTO_REFRESH_DELAYED_INTERVAL = 0.25;
        internal const double AUTO_REFRESH_PENDING_CHANGES_DELAYED_INTERVAL = 0.1;
        public const double PLUGIN_DELAYED_INITIALIZE_INTERVAL = 0.25;
        
        internal const double NOTIFICATION_CLEAR_INTERVAL = 5;

        internal const string PENDING_CHANGES_TABLE_SETTINGS_NAME = "{0}_PendingChangesTreeV3_{1}";
        internal const string GLUON_INCOMING_CHANGES_TABLE_SETTINGS_NAME = "{0}_GluonIncomingChangesTreeV2_{1}";
        internal const string GLUON_INCOMING_ERRORS_TABLE_SETTINGS_NAME = "{0}_GluonIncomingErrorsListV2_{1}";
        internal const string GLUON_UPDATE_REPORT_TABLE_SETTINGS_NAME = "{0}_GluonUpdateReportListV2_{1}";
        internal const string DEVELOPER_INCOMING_CHANGES_TABLE_SETTINGS_NAME = "{0}_DeveloperIncomingChangesTreeV3_{1}";
        internal const string DEVELOPER_UPDATE_REPORT_TABLE_SETTINGS_NAME = "{0}_DeveloperUpdateReportListV2_{1}";
        internal const string REPOSITORIES_TABLE_SETTINGS_NAME = "{0}_RepositoriesListV2_{1}";
        internal const string CHANGESETS_TABLE_SETTINGS_NAME = "{0}_ChangesetsListV3_{1}";
        internal const string CHANGESETS_DATE_FILTER_SETTING_NAME = "{0}_ChangesetsDateFilter_{1}";
        internal const string CHANGESETS_SHOW_CHANGES_SETTING_NAME = "{0}_ShowChanges_{1}";
        internal const string HISTORY_TABLE_SETTINGS_NAME = "{0}_HistoryListV2_{1}";
        internal const string BRANCHES_TABLE_SETTINGS_NAME = "{0}_BranchesListV2_{1}";
        internal const string BRANCHES_DATE_FILTER_SETTING_NAME = "{0}_BranchesDateFilter_{1}";

        internal const string PLASTIC_PLUGIN_IS_ENABLED_KEY_NAME = "{0}_PlasticPluginIsEnabled";
        internal const string SHOW_BRANCHES_VIEW_KEY_NAME = "{0}_ShowBranchesView";

        internal const string SHOW_NOTIFICATION_KEY_NAME = "ShowNotification";
        internal const string FORCE_CHECKOUT_KEY_NAME = "forceCheckoutPlasticSCM";

        internal static class ChangesetsColumns
        {
            internal const float CHANGESET_NUMBER_WIDTH = 80f;
            internal const float CHANGESET_NUMBER_MIN_WIDTH = 50f;
            internal const float CREATION_DATE_WIDTH = 150f;
            internal const float CREATION_DATE_MIN_WIDTH = 100f;
            internal const float CREATED_BY_WIDTH = 200f;
            internal const float CREATED_BY_MIN_WIDTH = 110f;
            internal const float COMMENT_WIDTH = 300f;
            internal const float COMMENT_MIN_WIDTH = 100f;
            internal const float BRANCH_WIDTH = 160f;
            internal const float BRANCH_MIN_WIDTH = 90f;
            internal const float REPOSITORY_WIDTH = 210f;
            internal const float REPOSITORY_MIN_WIDTH = 90f;
            internal const float GUID_WIDTH = 270f;
            internal const float GUID_MIN_WIDTH = 100f;
        }

        internal static class BranchesColumns
        {
            internal const float BRANCHES_NAME_WIDTH = 180f;
            internal const float BRANCHES_NAME_MIN_WIDTH = 70f;
            internal const float CREATION_DATE_WIDTH = 80f;
            internal const float CREATION_DATE_MIN_WIDTH = 60f;
            internal const float CREATEDBY_WIDTH = 200f;
            internal const float CREATEDBY_MIN_WIDTH = 110f;
            internal const float COMMENT_WIDTH = 300f;
            internal const float COMMENT_MIN_WIDTH = 100f;
            internal const float REPOSITORY_WIDTH = 180f;
            internal const float REPOSITORY_MIN_WIDTH = 90f;
            internal const float GUID_WIDTH = 270f;
            internal const float GUID_MIN_WIDTH = 100f;
        }
    }
}
