using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    // Assumption: Members are called from an OnGUI method ( otherwise style composition will fail)
    internal static class UnityStyles
    {
        internal static void Initialize(
            Action repaintPlasticWindow)
        {
            mRepaintPlasticWindow = repaintPlasticWindow;

            mLazyBackgroundStyles.Add(WarningMessage);
            mLazyBackgroundStyles.Add(SplitterIndicator);
            mLazyBackgroundStyles.Add(PlasticWindow.ActiveTabUnderline);
            mLazyBackgroundStyles.Add(Notification.GreenNotification);
            mLazyBackgroundStyles.Add(Notification.RedNotification);
            mLazyBackgroundStyles.Add(CancelButton);
            mLazyBackgroundStyles.Add(Inspector.HeaderBackgroundStyle);
            mLazyBackgroundStyles.Add(Inspector.DisabledHeaderBackgroundStyle);
        }

        internal static class Colors
        {
            internal static Color Transparent = new Color(255f / 255, 255f / 255, 255f / 255, 0f / 255);
            internal static Color GreenBackground = new Color(34f / 255, 161f / 255, 63f / 255);
            internal static Color GreenText = new Color(0f / 255, 100f / 255, 0f / 255);
            internal static Color Red = new Color(194f / 255, 51f / 255, 62f / 255);
            internal static Color Warning = new Color(255f / 255, 255f / 255, 176f / 255);
            internal static Color Splitter = new Color(100f / 255, 100f / 255, 100f / 255);
            internal static Color BarBorder = EditorGUIUtility.isProSkin ?
                (Color)new Color32(35, 35, 35, 255) :
                (Color)new Color32(153, 153, 153, 255);
#if UNITY_2019
            internal static Color InspectorHeaderBackground = EditorGUIUtility.isProSkin ?
                new Color(60f / 255, 60f / 255, 60f / 255) :
                new Color(203f / 255, 203f / 255, 203f / 255);
#else
            internal static Color InspectorHeaderBackground = Transparent;
#endif
#if UNITY_2019_1_OR_NEWER
            internal static Color InspectorHeaderBackgroundDisabled = EditorGUIUtility.isProSkin ?
                new Color(58f / 255, 58f / 255, 58f / 255) :
                new Color(199f / 255, 199f / 255, 199f / 255);
#else
            internal static Color InspectorHeaderBackgroundDisabled = EditorGUIUtility.isProSkin ?
                new Color(60f / 255, 60f / 255, 60f / 255) :
                new Color(210f / 255, 210f / 255, 210f / 255);
#endif
            internal static Color TabUnderline = new Color(58f / 255, 121f / 255, 187f / 255);
            internal static Color Link = new Color(0f, 120f / 255, 218f / 255);
            internal static Color SecondaryLabel = EditorGUIUtility.isProSkin ?
                new Color(196f / 255, 196f / 255, 196f / 255) :
                new Color(105f / 255, 105f / 255, 105f / 255);
            internal static Color BackgroundBar = EditorGUIUtility.isProSkin ? 
                new Color(35f / 255, 35f / 255, 35f / 255) :
                new Color(160f / 255, 160f / 255, 160f / 255);

            internal static Color TreeViewBackground = EditorGUIUtility.isProSkin ?
               new Color(48f / 255, 48f / 255, 48f / 255) :
               new Color(194f / 255, 194f / 255, 194f / 255);

            internal static Color CommentsBackground = EditorGUIUtility.isProSkin ?
               new Color(60f / 255, 60f / 255, 60f / 255) :
               new Color(160f / 255, 160f / 255, 160f / 255);

            internal static Color ColumnsBackground = EditorGUIUtility.isProSkin ?
              new Color(56f / 255, 56f / 255, 56f / 255) :
              new Color(221f / 255, 221f / 255, 221f / 255);

            internal static Color ToggleOffText = EditorGUIUtility.isProSkin ?
                new Color(131f / 255, 131f / 255, 131f / 255) :
                new Color(151f / 255, 151f / 255, 151f / 255);

            internal static Color ToggleHoverText = EditorGUIUtility.isProSkin ?
                new Color(129f / 255, 180f / 255, 255f / 255) :
                new Color(7f / 255, 68f / 255, 146f / 255);
        }

        internal static class HexColors
        {
            internal const string LINK_COLOR = "#0078DA";
        }

        internal static class Dialog
        {
            internal static readonly LazyStyle MessageTitle = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.boldLabel);
                style.contentOffset = new Vector2(0, -5);
                style.wordWrap = true;
                style.fontSize = MODAL_FONT_SIZE + 1;
                return style;
            });

            internal static readonly LazyStyle MessageText = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.wordWrap = true;
                style.fontSize = MODAL_FONT_SIZE;
                return style;
            });


            internal static readonly LazyStyle Toggle = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.boldLabel);
                style.fontSize = MODAL_FONT_SIZE;
                style.clipping = TextClipping.Overflow;
                return style;
            });

            internal static readonly LazyStyle RadioToggle = new LazyStyle(() =>
            {
                var radioToggleStyle = new GUIStyle(EditorStyles.radioButton);
                radioToggleStyle.fontSize = MODAL_FONT_SIZE;
                radioToggleStyle.clipping = TextClipping.Overflow;
                radioToggleStyle.font = EditorStyles.largeLabel.font;
                return radioToggleStyle;
            });

            internal static readonly LazyStyle Foldout = new LazyStyle(() =>
            {
                GUIStyle paragraphStyle = Paragraph;
                var foldoutStyle = new GUIStyle(EditorStyles.foldout);
                foldoutStyle.fontSize = MODAL_FONT_SIZE;
                foldoutStyle.font = paragraphStyle.font;
                return foldoutStyle;
            });

            internal static readonly LazyStyle EntryLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.textField);
                style.wordWrap = true;
                style.fontSize = MODAL_FONT_SIZE;
                return style;
            });

            internal static readonly LazyStyle AcceptButtonText = new LazyStyle(() =>
            {
                var style = new GUIStyle(GetEditorSkin().GetStyle("WhiteLabel"));
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = MODAL_FONT_SIZE + 1;
                style.normal.background = null;
                return style;
            });

            internal static readonly LazyStyle NormalButton = new LazyStyle(() =>
            {
                var style = new GUIStyle(GetEditorSkin().button);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = MODAL_FONT_SIZE;
                return style;
            });
        }

        internal static class Tree
        {
            internal static readonly LazyStyle IconStyle = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.largeLabel);
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            });

            internal static readonly LazyStyle Label = new LazyStyle(() =>
            {
                var style = new GUIStyle(TreeView.DefaultStyles.label);
                style.fontSize = 11;
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            });

            internal static readonly LazyStyle SecondaryLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(TreeView.DefaultStyles.label);
                style.fontSize = 11;
                style.alignment = TextAnchor.MiddleLeft;

                style.active = new GUIStyleState() { textColor = Colors.SecondaryLabel };
                style.focused = new GUIStyleState() { textColor = Colors.SecondaryLabel };
                style.hover = new GUIStyleState() { textColor = Colors.SecondaryLabel };
                style.normal = new GUIStyleState() { textColor = Colors.SecondaryLabel };

                return style;
            });

            internal static readonly LazyStyle InfoLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(MultiColumnHeader.DefaultStyles.columnHeader);
                return style;
            });

            internal static readonly LazyStyle SecondaryBoldLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(SecondaryLabel);
                style.fontStyle = FontStyle.Bold;
                return style;
            });

            internal static readonly LazyStyle RedLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(Label);
                style.active = new GUIStyleState() { textColor = Colors.Red };
                style.focused = new GUIStyleState() { textColor = Colors.Red };
                style.hover = new GUIStyleState() { textColor = Colors.Red };
                style.normal = new GUIStyleState() { textColor = Colors.Red };
                return style;
            });

            internal static readonly LazyStyle GreenLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(Label);
                style.active = new GUIStyleState() { textColor = Colors.GreenText };
                style.focused = new GUIStyleState() { textColor = Colors.GreenText };
                style.hover = new GUIStyleState() { textColor = Colors.GreenText };
                style.normal = new GUIStyleState() { textColor = Colors.GreenText };
                return style;
            });

            internal static readonly LazyStyle BoldLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(TreeView.DefaultStyles.boldLabel);
                style.fontSize = 11;
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            });

            internal static readonly LazyStyle LabelRightAligned = new LazyStyle(() =>
            {
                var style = new GUIStyle(TreeView.DefaultStyles.label);
                style.fontSize = 11;
                style.alignment = TextAnchor.MiddleRight;
                return style;
            });

            internal static readonly LazyStyle SecondaryLabelRightAligned = new LazyStyle(() =>
            {
                var style = new GUIStyle(SecondaryLabel);
                style.alignment = TextAnchor.MiddleRight;
                return style;
            });

            internal static readonly LazyStyle SecondaryLabelBoldRightAligned = new LazyStyle(() =>
            {
                var style = new GUIStyle(SecondaryLabelRightAligned);
                style.fontStyle = FontStyle.Bold;
                return style;
            });

            internal static readonly LazyStyle StatusLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 14;
                style.padding = new RectOffset(
                    0, 0, 
                    UnityConstants.TREEVIEW_STATUS_CONTENT_PADDING, UnityConstants.TREEVIEW_STATUS_CONTENT_PADDING);
                style.stretchWidth = false;
                return style;
            });

            internal static readonly LazyStyle Columns = new LazyStyle(() =>
            {
                var style = new GUIStyle();
                style.normal.background = Images.GetColumnsBackgroundTexture();
                return style;
            });
        }

        public static class Inspector
        {
            public static readonly LazyStyle HeaderBackgroundStyle = new LazyStyle(() =>
            {
                return CreateUnderlineStyle(
                    Colors.InspectorHeaderBackground,
                    UnityConstants.INSPECTOR_ACTIONS_HEADER_BACK_RECTANGLE_HEIGHT);
            });

            public static readonly LazyStyle DisabledHeaderBackgroundStyle = new LazyStyle(() =>
            {
                return CreateUnderlineStyle(
                    Colors.InspectorHeaderBackgroundDisabled,
                    UnityConstants.INSPECTOR_ACTIONS_HEADER_BACK_RECTANGLE_HEIGHT);
            });
        }

        internal static class ProjectSettings
        {
            internal static readonly LazyStyle ToggleOn = new LazyStyle(() =>
            {
                GUIStyle result = new GUIStyle(Toggle);
                result.hover.textColor = Colors.ToggleHoverText;
                return result;
            });

            static readonly LazyStyle Toggle = new LazyStyle(() =>
            {
                GUIStyle result = new GUIStyle(EditorStyles.miniButton);
                result.fixedHeight = 22;
                result.fixedWidth = 85;
                result.fontSize = 12;
                return result;
            });
        }

        internal static class PlasticWindow
        {
            internal static readonly LazyStyle TabButton = new LazyStyle(() =>
            {
                GUIStyle result = new GUIStyle(EditorStyles.label);
                result.padding = EditorStyles.toolbarButton.padding;
                result.margin = EditorStyles.toolbarButton.margin;
                result.contentOffset = EditorStyles.toolbarButton.contentOffset;
                result.alignment = EditorStyles.toolbarButton.alignment;
                result.fixedHeight = EditorStyles.toolbarButton.fixedHeight;
                return result;
            });

            internal static readonly LazyStyle ActiveTabUnderline = new LazyStyle(() =>
            {
                return CreateUnderlineStyle(
                    Colors.TabUnderline,
                    UnityConstants.ACTIVE_TAB_UNDERLINE_HEIGHT);
            });
        }

        internal static class StatusBar
        {
            internal static readonly LazyStyle Icon = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.padding.left = 0;
                style.padding.right = 0;
                return style;
            });

            internal static readonly LazyStyle Label = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                return style;
            });

            internal static readonly LazyStyle LinkLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.linkLabel);
                style.padding = EditorStyles.label.padding;
                return style;
            });

            internal static readonly LazyStyle NotificationLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fontStyle = FontStyle.Bold;
                return style;
            });

            internal static readonly LazyStyle Button = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.miniButtonLeft);
                style.fixedWidth = 60;
                return style;
            });

            internal static readonly LazyStyle NotificationPanel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.helpBox);
                style.fixedHeight = 24;
                return style;
            });

            internal static readonly LazyStyle NotificationPanelCloseButton = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fixedHeight = 16;
                style.fixedWidth = 16;
                return style;
            });
        }

        internal static class DiffPanel
        {
            internal static readonly LazyStyle HeaderLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fontSize = 10;
                style.fontStyle = FontStyle.Bold;
#if UNITY_2019_1_OR_NEWER
                style.contentOffset = new Vector2(0, 1.5f);
#endif
                return style;
            });
        }

        internal static class PendingChangesTab
        {
            internal static readonly LazyStyle CommentPlaceHolder = new LazyStyle(() =>
            {
                var style = new GUIStyle();
                style.normal = new GUIStyleState() { textColor = Color.gray };
                style.padding = new RectOffset(7, 0, 4, 0);
                return style;
            });

            internal static readonly LazyStyle CommentTextArea = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.textArea);
                style.margin = new RectOffset(7, 4, 0, 0);
                style.padding = new RectOffset(0, 0, 4, 0);

                return style;
            });

            internal static readonly LazyStyle CommentWarningIcon = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fontSize = 10;
#if !UNITY_2019_1_OR_NEWER
                style.margin = new RectOffset(0, 0, 0, 0);
#endif
                return style;
            });

            internal static readonly LazyStyle HeaderLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fontSize = 10;
                style.fontStyle = FontStyle.Bold;
#if UNITY_2019_1_OR_NEWER
                style.contentOffset = new Vector2(0, 1.5f);
#endif
                return style;
            });

            internal static readonly LazyStyle Comment = new LazyStyle(() =>
            {
                var style = new GUIStyle();
                style.normal.background = Images.GetCommentBackgroundTexture();
                return style;
            });

            internal static readonly GUIStyle DefaultMultiColumHeader = MultiColumnHeader.DefaultStyles.background;
        }

        internal static class IncomingChangesTab
        {
            internal static readonly LazyStyle PendingConflictsLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fontSize = 11;
                style.padding.top = 2;
                style.fontStyle = FontStyle.Bold;
                return style;
            });

            internal static readonly LazyStyle RedPendingConflictsOfTotalLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(PendingConflictsLabel);
                style.normal = new GUIStyleState() { textColor = Colors.Red };
                return style;
            });

            internal static readonly LazyStyle GreenPendingConflictsOfTotalLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(PendingConflictsLabel);
                style.normal = new GUIStyleState() { textColor = Colors.GreenText };
                return style;
            });

            internal static readonly LazyStyle ChangesToApplySummaryLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fontSize = 11;
                style.padding.top = 2;
                return style;
            });

            internal readonly static LazyStyle HeaderWarningLabel
                = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fontSize = 11;
#if !UNITY_2019_1_OR_NEWER
                style.margin = new RectOffset(0, 0, 0, 0);
#endif
                return style;
            });
        }

        internal static class ChangesetsTab
        {
            internal static readonly LazyStyle HeaderLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fontSize = 10;
                style.fontStyle = FontStyle.Bold;
#if UNITY_2019_1_OR_NEWER
                style.contentOffset = new Vector2(0, 1.5f);
#endif
                return style;
            });
        }

        internal static class HistoryTab
        {
            internal static readonly LazyStyle HeaderLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fontSize = 10;
                style.fontStyle = FontStyle.Bold;
#if UNITY_2019_1_OR_NEWER
                style.contentOffset = new Vector2(0, 1.5f);
#endif
                return style;
            });
        }

        internal static class DirectoryConflictResolution
        {
            internal readonly static LazyStyle WarningLabel
                = new LazyStyle(() =>
                {
                    var style = new GUIStyle(EditorStyles.label);
                    style.alignment = TextAnchor.MiddleLeft;
#if !UNITY_2019_1_OR_NEWER
                    style.margin = new RectOffset(0, 0, 0, 0);
#endif
                    return style;
                });
        }

        internal static class Notification
        {
            internal static readonly LazyStyle Label = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
#if !UNITY_2019_1_OR_NEWER
                style.fontSize = 10;
#endif
                style.normal = new GUIStyleState() { textColor = Color.white };
                return style;
            });

            internal static readonly LazyStyle GreenNotification = new LazyStyle(() =>
            {
                var style = new GUIStyle();
                style.wordWrap = true;
                style.margin = new RectOffset();
                style.padding = new RectOffset(4, 4, 2, 2);
                style.stretchWidth = true;
                style.stretchHeight = true;
                style.alignment = TextAnchor.UpperLeft;

                var bg = new Texture2D(1, 1);
                bg.SetPixel(0, 0, Colors.GreenBackground);
                bg.Apply();
                style.normal.background = bg;
                return style;
            });

            internal static readonly LazyStyle RedNotification = new LazyStyle(() =>
            {
                var style = new GUIStyle();
                style.wordWrap = true;
                style.margin = new RectOffset();
                style.padding = new RectOffset(4, 4, 2, 2);
                style.stretchWidth = true;
                style.stretchHeight = true;
                style.alignment = TextAnchor.UpperLeft;

                var bg = new Texture2D(1, 1);
                bg.SetPixel(0, 0, Colors.Red);
                bg.Apply();
                style.normal.background = bg;
                return style;
            });
        }

        internal static class DirectoryConflicts
        {
            internal readonly static LazyStyle TitleLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.largeLabel);
                RectOffset margin = new RectOffset(
                    style.margin.left,
                    style.margin.right,
                    style.margin.top - 1,
                    style.margin.bottom);
                style.margin = margin;
                style.fontStyle = FontStyle.Bold;
                return style;
            });

            internal readonly static LazyStyle BoldLabel = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fontStyle = FontStyle.Bold;
                return style;
            });

            internal readonly static LazyStyle FileNameTextField = new LazyStyle(() =>
            {
                var style = new GUIStyle(EditorStyles.textField);
                RectOffset margin = new RectOffset(
                    style.margin.left,
                    style.margin.right,
                    style.margin.top + 2,
                    style.margin.bottom);
                style.margin = margin;
                return style;
            });
        }

        internal static readonly LazyStyle ActionToolbar = new LazyStyle(() =>
        {
            var style = new GUIStyle(EditorStyles.toolbar);
            style.fixedHeight = 40f;
            style.padding = new RectOffset(5, 5, 5, 5);
            return style;
        });

        internal static readonly LazyStyle SplitterIndicator = new LazyStyle(() =>
        {
            return CreateUnderlineStyle(
                Colors.Splitter,
                UnityConstants.SPLITTER_INDICATOR_HEIGHT);
        });

        internal static readonly LazyStyle HelpBoxLabel = new LazyStyle(() =>
        {
            var style = new GUIStyle(EditorStyles.label);
            style.fontSize = 10;
            style.wordWrap = true;
            return style;
        });

        internal static readonly LazyStyle ProgressLabel = new LazyStyle(() =>
        {
            var style = new GUIStyle(EditorStyles.label);
            style.fontSize = 10;
#if !UNITY_2019_1_OR_NEWER
            style.margin = new RectOffset(0, 0, 0, 0);
#endif
            return style;
        });

        internal static readonly LazyStyle TextFieldWithWrapping = new LazyStyle(() =>
        {
            var style = new GUIStyle(GetEditorSkin().textArea);
            style.normal = new GUIStyleState() {
                textColor = GetEditorSkin().textArea.normal.textColor,
                background = Images.GetTreeviewBackgroundTexture()
            };
                
            style.wordWrap = true;
            return style;
        });

        internal static readonly LazyStyle Search = new LazyStyle(() =>
        {
            var style = new GUIStyle();
            style.normal = new GUIStyleState() { textColor = Color.gray };
            style.padding = new RectOffset(18, 0, 0, 0);
            return style;
        });

        internal static readonly LazyStyle WarningMessage = new LazyStyle(() =>
        {
            var style = new GUIStyle(GetEditorSkin().box);
            style.wordWrap = true;
            style.margin = new RectOffset();
            style.padding = new RectOffset(8, 8, 6, 6);
            style.stretchWidth = true;
            style.alignment = TextAnchor.UpperLeft;

            var bg = new Texture2D(1, 1);
            bg.SetPixel(0, 0, Colors.Warning);
            bg.Apply();
            style.normal.background = bg;
            return style;
        });

        internal static readonly LazyStyle CancelButton = new LazyStyle(() =>
        {
            var normalIcon = Images.GetImage(Images.Name.IconCloseButton);
            var pressedIcon = Images.GetImage(Images.Name.IconPressedCloseButton);

            var style = new GUIStyle();
            style.normal = new GUIStyleState() { background = normalIcon };
            style.onActive = new GUIStyleState() { background = pressedIcon };
            style.active = new GUIStyleState() { background = pressedIcon };
            return style;
        });

        internal static readonly LazyStyle MiniToggle = new LazyStyle(() =>
        {
            var style = new GUIStyle(EditorStyles.boldLabel);
            style.fontSize = MODAL_FONT_SIZE - 1;
            style.clipping = TextClipping.Overflow;
            return style;
        });

        internal static readonly LazyStyle Paragraph = new LazyStyle(() =>
        {
            var style = new GUIStyle(EditorStyles.largeLabel);
            style.wordWrap = true;
            style.richText = true;
            style.fontSize = MODAL_FONT_SIZE;
            return style;
        });

        static GUISkin GetEditorSkin()
        {
            GUISkin editorSkin = null;
            if (EditorGUIUtility.isProSkin)
                editorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
            else
                editorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            return editorSkin;
        }

        static GUIStyle CreateUnderlineStyle(Color color, int height)
        {
            GUIStyle style = new GUIStyle();

            Texture2D pixel = new Texture2D(1, height);

            for (int i = 0; i < height; i++)
                pixel.SetPixel(0, i, color);

            pixel.wrapMode = TextureWrapMode.Repeat;
            pixel.Apply();

            style.normal.background = pixel;
            style.fixedHeight = height;

            return style;
        }

        static void EnsureBackgroundStyles(LazyStyle lazy)
        {
            // The editor cleans the GUIStyleState.background property
            // when entering the edit mode (exiting the play mode)
            // and also in other situations (e.g when you use Zoom app)
            // Because of this, we have to reset them in order to
            // re-instantiate them the next time they're used

            if (!mLazyBackgroundStyles.Contains(lazy))
                return;

            bool needsRepaint = false;

            foreach (LazyStyle style in mLazyBackgroundStyles)
            {
                if (!style.IsInitialized)
                    continue;

                if (style.Value.normal.background != null)
                    continue;

                style.Reset();

                needsRepaint = true;
            }

            if (!needsRepaint)
                return;

            if (mRepaintPlasticWindow != null)
                mRepaintPlasticWindow();
        }

        static List<LazyStyle> mLazyBackgroundStyles = new List<LazyStyle>();

        internal class LazyStyle
        {
            internal bool IsInitialized { get; private set; }

            internal LazyStyle(Func<GUIStyle> builder)
            {
                mBuilder = builder;
                IsInitialized = false;
            }
            internal GUIStyle Value { get; private set; }

            internal void Reset()
            {
                IsInitialized = false;
            }

            public static implicit operator GUIStyle(LazyStyle lazy)
            {
                if (lazy.IsInitialized)
                {
                    EnsureBackgroundStyles(lazy);
                    return lazy.Value;
                }

                lazy.Value = lazy.mBuilder();
                lazy.IsInitialized = true;
                return lazy.Value;
            }

            readonly Func<GUIStyle> mBuilder;
        }

        static Action mRepaintPlasticWindow;

        const int MODAL_FONT_SIZE = 13;
    }
}
