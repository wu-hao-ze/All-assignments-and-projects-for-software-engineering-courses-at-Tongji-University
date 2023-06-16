using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class LudiqStyles
    {
        static LudiqStyles()
        {
            // General

            centeredLabel = new GUIStyle(EditorStyles.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.margin = new RectOffset(0, 0, 5, 5);
            centeredLabel.wordWrap = true;

            horizontalSeparator = ColorPalette.unityBackgroundVeryDark.CreateBackground();
            horizontalSeparator.fixedHeight = 1;
            horizontalSeparator.stretchWidth = true;

            verticalSeparator = ColorPalette.unityBackgroundVeryDark.CreateBackground();
            verticalSeparator.fixedWidth = 1;
            verticalSeparator.stretchHeight = true;

            expandedTooltip = new GUIStyle(EditorStyles.label);
            expandedTooltip.normal.textColor = ColorPalette.unityForegroundDim;
            expandedTooltip.wordWrap = true;
            expandedTooltip.fontSize = 10;
            expandedTooltip.padding = new RectOffset(2, 5, 0, 10);

            paddedButton = new GUIStyle("Button");
            paddedButton.padding = new RectOffset(10, 10, 5, 5);

            textAreaWordWrapped = new GUIStyle(EditorStyles.textArea);
            textAreaWordWrapped.wordWrap = true;

            spinnerButton = new GUIStyle("MiniToolbarButton");
            spinnerButton.padding = new RectOffset(0, 0, 0, 0);
            spinnerButton.imagePosition = ImagePosition.ImageOnly;
            spinnerButton.alignment = TextAnchor.MiddleCenter;
            spinnerButton.fixedWidth = 0;
            spinnerButton.fixedHeight = 0;

            spinnerDownArrow = new GUIStyle(LudiqGUIUtility.newSkin ? "IN MinMaxStateDropDown" : "IN DropDown").normal.background;

            // Headers

            headerBackground = new GUIStyle("IN BigTitle");
            headerBackground.margin = new RectOffset(0, 0, 0, 5);

            // Show smaller icons on high DPI displays,
            // and crisp 32x icons on standard DPI displays

            if (EditorGUIUtility.pixelsPerPoint >= 2)
            {
                headerBackground.padding = new RectOffset(8, 8, 8, 9);
            }
            else
            {
                headerBackground.padding = new RectOffset(8, 6, 6, 7);
            }

            headerTitle = new GUIStyle(EditorStyles.label);
            headerTitle.fontSize = 13;
            headerTitle.wordWrap = true;

            headerSummary = new GUIStyle(EditorStyles.label);
            headerSummary.wordWrap = true;

            headerIcon = new GUIStyle();

            if (EditorGUIUtility.pixelsPerPoint >= 2)
            {
                headerIcon.fixedWidth = 20;
                headerIcon.fixedHeight = 20;
                headerIcon.margin = new RectOffset(0, 8, 3, 0);
            }
            else
            {
                headerIcon.fixedWidth = 32;
                headerIcon.fixedHeight = 32;
                headerIcon.margin = new RectOffset(0, 6, 3, 0);
            }

            headerTitleField = new GUIStyle(EditorStyles.textField);
            headerTitleField.fontSize = 13;
            headerTitleField.fixedHeight = 19;

            headerTitleFieldHidable = new GUIStyle(headerTitleField);
            headerTitleFieldHidable.hover.background = headerTitleFieldHidable.normal.background;
            headerTitleFieldHidable.normal.background = ColorPalette.transparent.GetPixel();

            headerTitlePlaceholder = new GUIStyle(EditorStyles.label);
            headerTitlePlaceholder.normal.textColor = EditorStyles.centeredGreyMiniLabel.normal.textColor;
            headerTitlePlaceholder.padding = headerTitleField.padding;
            headerTitlePlaceholder.fontSize = headerTitleField.fontSize;

            headerSummaryField = new GUIStyle(EditorStyles.textArea);

            headerSummaryFieldHidable = new GUIStyle(headerSummaryField);
            headerSummaryFieldHidable.hover.background = headerSummaryFieldHidable.normal.background;
            headerSummaryFieldHidable.normal.background = ColorPalette.transparent.GetPixel();

            headerSummaryPlaceholder = new GUIStyle(EditorStyles.label);
            headerSummaryPlaceholder.normal.textColor = EditorStyles.centeredGreyMiniLabel.normal.textColor;
            headerSummaryPlaceholder.padding = EditorStyles.textField.padding;

            // Lists

            listBackground = ColorPalette.unityBackgroundLight.CreateBackground();

            listRow = new GUIStyle();
            listRow.fontSize = 13;
            listRow.richText = true;
            listRow.alignment = TextAnchor.MiddleRight;
            listRow.padding = new RectOffset(18, 8, 10, 10);

            var normalBackground = ColorPalette.transparent.GetPixel();
            var selectedBackground = ColorPalette.unitySelectionHighlight.GetPixel();
            var normalForeground = ColorPalette.unityForeground;
            var selectedForeground = ColorPalette.unityForegroundSelected;

            listRow.normal.background = normalBackground;
            listRow.normal.textColor = normalForeground;
            listRow.onNormal.background = selectedBackground;
            listRow.onNormal.textColor = selectedForeground;

            listRow.active.background = normalBackground;
            listRow.active.textColor = normalForeground;
            listRow.onActive.background = selectedBackground;
            listRow.onActive.textColor = selectedForeground;

            listRow.border = new RectOffset(1, 1, 1, 1);

            // Toolbars

            toolbarBackground = new GUIStyle(EditorStyles.toolbar);
            toolbarButton = new GUIStyle(EditorStyles.toolbarButton);
            toolbarButton.alignment = TextAnchor.MiddleCenter;
            toolbarButton.padding.right -= 2;
            toolbarPopup = new GUIStyle(EditorStyles.toolbarPopup);

            toolbarBreadcrumbRoot = new GUIStyle("GUIEditor.BreadcrumbLeft");
            toolbarBreadcrumbRoot.alignment = TextAnchor.MiddleCenter;
            toolbarBreadcrumbRoot.padding.bottom++;
            toolbarBreadcrumbRoot.fontSize = 9;
            toolbarBreadcrumbRoot.margin.left = 0;

            toolbarBreadcrumb = new GUIStyle("GUIEditor.BreadcrumbMid");
            toolbarBreadcrumb.alignment = TextAnchor.MiddleCenter;
            toolbarBreadcrumb.padding.bottom++;
            toolbarBreadcrumb.fontSize = 9;

            toolbarLabel = new GUIStyle(EditorStyles.label);
            toolbarLabel.alignment = TextAnchor.MiddleCenter;
            toolbarLabel.padding = new RectOffset(2, 2, 2, 2);
            toolbarLabel.fontSize = 9;

            // Windows

            windowHeaderBackground = new GUIStyle("IN BigTitle");
            windowHeaderBackground.margin = new RectOffset(0, 0, 0, 0);
            windowHeaderBackground.padding = new RectOffset(10, 10, 20, 20);

            windowHeaderTitle = new GUIStyle(EditorStyles.label);
            windowHeaderTitle.padding = new RectOffset(0, 0, 0, 0);
            windowHeaderTitle.margin = new RectOffset(0, 0, 0, 0);
            windowHeaderTitle.alignment = TextAnchor.MiddleCenter;
            windowHeaderTitle.fontSize = 14;

            windowHeaderIcon = new GUIStyle();
            windowHeaderIcon.imagePosition = ImagePosition.ImageOnly;
            windowHeaderIcon.alignment = TextAnchor.MiddleCenter;
            windowHeaderIcon.fixedWidth = IconSize.Medium;
            windowHeaderIcon.fixedHeight = IconSize.Medium;

            windowHeaderTitle.fixedHeight = headerIcon.fixedHeight;

            windowBackground = ColorPalette.unityBackgroundMid.CreateBackground();
        }

        // General
        public static readonly GUIStyle centeredLabel;
        public static readonly GUIStyle horizontalSeparator;
        public static readonly GUIStyle verticalSeparator;
        public static readonly GUIStyle expandedTooltip;
        public static readonly GUIStyle paddedButton;
        public static readonly float compactHorizontalSpacing = 2;
        public static readonly GUIStyle textAreaWordWrapped;
        public static readonly GUIStyle spinnerButton;
        public static readonly Texture2D spinnerDownArrow;

        // Headers
        public static readonly GUIStyle headerBackground;
        public static readonly GUIStyle headerIcon;
        public static readonly GUIStyle headerTitle;
        public static readonly GUIStyle headerTitleField;
        public static readonly GUIStyle headerTitleFieldHidable;
        public static readonly GUIStyle headerTitlePlaceholder;
        public static readonly GUIStyle headerSummary;
        public static readonly GUIStyle headerSummaryField;
        public static readonly GUIStyle headerSummaryFieldHidable;
        public static readonly GUIStyle headerSummaryPlaceholder;

        // Lists
        public static readonly GUIStyle listRow;
        public static readonly GUIStyle listBackground;

        // Toolbars
        public static readonly GUIStyle toolbarBackground;
        public static readonly GUIStyle toolbarButton;
        public static readonly GUIStyle toolbarPopup;
        public static readonly GUIStyle toolbarBreadcrumbRoot;
        public static readonly GUIStyle toolbarBreadcrumb;
        public static readonly GUIStyle toolbarLabel;

        // Windows
        public static readonly GUIStyle windowHeaderBackground;
        public static readonly GUIStyle windowHeaderTitle;
        public static readonly GUIStyle windowHeaderIcon;
        public static readonly GUIStyle windowBackground;
        public static readonly float spaceBetweenWindowHeaderIconAndTitle = 14;
    }
}
