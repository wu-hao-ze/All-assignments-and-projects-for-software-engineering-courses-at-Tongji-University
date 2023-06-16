using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(StickyNote))]
    public class StickyNoteWidget : GraphElementWidget<ICanvas, StickyNote>
    {
        public StickyNoteWidget(ICanvas canvas, StickyNote stickyNote) : base(canvas, stickyNote)
        {
            minResizeSize = new Vector2(64, 64);
            resizeInnerOffset.top = 0;
        }

        #region Model

        private StickyNote sticky => element;

        public override void CacheItem()
        {
            base.CacheItem();

            labelContent.text = sticky.title;
        }

        #endregion


        #region Lifecycle

        public override bool foregroundRequiresInput => true;

        #endregion


        #region Contents

        private static readonly GUIContent maxHeadLabelSizeContent = new GUIContent("M");

        private readonly GUIContent labelContent = new GUIContent();

        private Vector2 scrollPosition = Vector2.zero;

        private static Color _transparentWhite = new Color(1, 1, 1, .5f);

        #endregion


        #region Positioning

        protected override bool snapToGrid => BoltCore.Configuration.snapToGrid;

        public override float zIndex
        {
            get { return float.MaxValue; }
            set { }
        }

        public override Rect position
        {
            get { return sticky.position; }
            set { sticky.position = value; }
        }

        public Rect titlePosition { get; private set; }

        public Rect bodyPosition { get; private set; }

        public override Rect hotArea => headerPosition;

        public override void CachePosition()
        {
            AdjustLabelFontSize();

            //HeaderPosition is the draggable area, so the entire Sticky Note in this case.
            headerPosition = position;

            titlePosition = new Rect
            (
                position.x + Styles.title.margin.left,
                position.y + Styles.title.margin.top,
                position.width - Styles.body.margin.left,
                Styles.sticky.border.top
            );

            bodyPosition = new Rect
            (
                position.x + Styles.body.margin.left,
                position.y + Styles.body.margin.top + Styles.headerHeight,
                position.width - Styles.body.margin.left,
                position.height - Styles.body.margin.bottom - Styles.headerHeight
            );
        }

        public override void OnViewportChange()
        {
            base.OnViewportChange();

            Reposition(); // The label position is dependant on the zoom factor
        }

        private void AdjustLabelFontSize()
        {
            Styles.title.fontSize = Mathf.RoundToInt(Styles.headerFontSize / graph.zoom);
        }

        #endregion


        #region Drawing

        static void DrawBox(Rect rect, int thickness, Color color)
        {
            var left = new Rect(rect.x, rect.y, thickness, rect.height);
            var right = new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height);
            var top = new Rect(rect.x, rect.y, rect.width, thickness);
            var bottom = new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness);

            EditorGUI.DrawRect(left, color);
            EditorGUI.DrawRect(right, color);
            EditorGUI.DrawRect(top, color);
            EditorGUI.DrawRect(bottom, color);
        }

        public override void DrawForeground()
        {
            AdjustLabelFontSize();

            var displayMoveArrowCursor = true;

            var selected = selection.Contains(sticky);
            if (!selected)
            {
                focusOnBody = false;
                focusOnTitle = false;
            }

            //Sticky background
            if (e.IsRepaint)
            {
                using (LudiqGUI.color.Override(StickyNote.GetStickyColor(sticky.colorTheme)))

                {
                    Styles.sticky.Draw(position, false, false, selected, false);
                }


                int borderThickness = 0;
                //Focus border
                if (position.Contains(mousePosition))
                {
                    borderThickness++;
                }

                if (selected)
                {
                    borderThickness++;
                }

                if (borderThickness > 0)
                {
                    DrawBox(position, borderThickness, Color.cyan);
                }
            }

            //Title section
            using (new EditorGUI.DisabledScope(!focusOnTitle))
            {
                GUI.SetNextControlName(item.guid + "title");

                var color = StickyNote.GetFontColor(sticky.colorTheme);

                var titleBanner = titlePosition.position;
                var titleSize = Styles.title.CalcSize(new GUIContent(sticky.title));

                titleSize.y -= Styles.title.padding.top;
                titleBanner.y += Styles.title.padding.top;
                var titleBannerRect = new Rect(titleBanner, titleSize);

                //Add semi transparent text background for Pro due to dark on dark background
                if (EditorGUIUtility.isProSkin && color == Color.black)
                {
                    //Enable when title is longer than width of the sticky
                    if (bodyPosition.width < titleSize.x)
                    {
                        EditorGUI.DrawRect(titleBannerRect, _transparentWhite);
                    }
                }

                //Draw focus border when mouse over text
                if (bodyPosition.width > titleSize.x)
                {
                    //Use the title area width if the text is under the sticky box width
                    titleBannerRect.width = titlePosition.width - Styles.title.margin.left;
                }

                if (titleBannerRect.Contains(mousePosition))
                {
                    if (focusOnTitle)
                    {
                        displayMoveArrowCursor = false;
                        window.AddCursorRect(titleBannerRect, MouseCursor.Text);
                    }
                    DrawBox(titleBannerRect, 1, Color.cyan);
                }

                if (!focusOnTitle)
                    color.a += 1; //Offset the disable color
                using (LudiqGUI.color.Override(color))
                {
                    if (focusOnTitle)
                    {
                        sticky.title = EditorGUI.TextField(titlePosition, GUIContent.none, sticky.title,
                            Styles.title);
                    }
                    else
                    {
                        EditorGUI.LabelField(titlePosition, sticky.title, Styles.title);
                    }
                }
            }

            //Body section
            var bodyBox = bodyPosition;
            bodyBox.width -= Styles.title.margin.left;

            var scrollBox = bodyBox;

            var bodyContentHeight = Styles.body.CalcHeight(new GUIContent(sticky.body), bodyBox.width);
            var needScrollbar = bodyContentHeight > bodyBox.height;

            if (needScrollbar)
                bodyBox.width -= 12;

            var innerScrollboxRect = bodyBox;
            innerScrollboxRect.height = needScrollbar ?
                Styles.body.CalcHeight(new GUIContent(sticky.body), bodyBox.width) :
                bodyContentHeight;
            innerScrollboxRect.width -= 1;

            //Body mouse over focus box
            if (bodyBox.Contains(mousePosition))
            {
                if (focusOnBody)
                {
                    displayMoveArrowCursor = false;
                    window.AddCursorRect(bodyBox, MouseCursor.Text);
                }
                DrawBox(bodyBox, 1, Color.cyan);
            }
            else
            {
                if (needScrollbar && bodyPosition.Contains(mousePosition))
                {
                    //Cursor on scrollbar
                    displayMoveArrowCursor = false;
                }
            }

            scrollPosition = GUI.BeginScrollView(scrollBox, scrollPosition, innerScrollboxRect);
            using (new EditorGUI.DisabledScope(!focusOnBody))
            {
                GUI.SetNextControlName(item.guid + "body");
                var color = StickyNote.GetFontColor(sticky.colorTheme);

                if (!focusOnBody)
                    color.a += 1f; //Offset the disable color
                using (LudiqGUI.color.Override(color))
                {
                    if (focusOnBody)
                    {
                        sticky.body = GUI.TextArea(innerScrollboxRect, sticky.body, Styles.body);
                    }
                    else
                    {
                        EditorGUI.LabelField(bodyBox, sticky.body, Styles.body);
                    }
                }
            }

            GUI.EndScrollView();

            if (displayMoveArrowCursor && isMouseOverHeaderArea)
            {
                window.AddCursorRect(hotArea, MouseCursor.MoveArrow);
            }
        }
        #endregion


        #region Deleting

        public override bool canDelete => true;

        #endregion


        #region Layouting

        public override bool canAlignAndDistribute => false;

        #endregion


        #region Resizing

        public override bool canResizeHorizontal => true;

        public override bool canResizeVertical => true;

        #endregion


        #region Selecting

        private bool focusOnTitle;

        private bool focusOnBody;
        public override bool canSelect => true;

        protected override void OnDoubleClick()
        {
            focusOnTitle = false;
            focusOnBody = false;

            if (titlePosition.Contains(mousePosition))
            {
                focusOnTitle = true;
                EditorGUI.FocusTextInControl(item.guid + "title");
            }

            if (bodyPosition.Contains(mousePosition))
            {
                focusOnBody = true;
                EditorGUI.FocusTextInControl(item.guid + "body");
            }
        }

        #endregion


        #region Dragging

        public override bool canDrag => true;

        #endregion


        public static class Styles
        {
            static Styles()
            {
                sticky = new GUIStyle();
                sticky.normal.background = Texture2D.whiteTexture;
                sticky.onNormal.background = sticky.normal.background;

                sticky.onActive.background = Texture2D.grayTexture;
                sticky.border = new RectOffset(16, 16, 25, 16);

                title = new GUIStyle();
                title.normal.textColor = new Color(1, 1, 1, 0.75f);
                title.alignment = TextAnchor.UpperLeft;
                title.padding = new RectOffset(0, 5, 5, 0);
                title.margin = new RectOffset(10, 0, 0, 0);

                body = new GUIStyle();
                body.normal.textColor = new Color(1, 1, 1, 0.75f);
                body.alignment = TextAnchor.UpperLeft;
                body.wordWrap = true;
                body.padding = new RectOffset(0, 5, 0, 0);
                body.margin = new RectOffset(10, 0, 0, 0);
                body.focused = new GUIStyleState() { };
                body.focused.textColor = Color.cyan;
            }

            public static readonly float headerFontSize = 14;

            public static readonly GUIStyle sticky;

            public static readonly GUIStyle title;

            public static readonly GUIStyle body;

            public static float headerHeight => sticky.border.top;
        }
    }
}
