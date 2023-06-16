using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class SidebarPanel
    {
        public SidebarPanel(ISidebarPanelContent content)
        {
            Ensure.That(nameof(content)).IsNotNull(content);

            this.content = content;
        }

        [DoNotSerialize]
        public Sidebars sidebars { get; set; }

        [DoNotSerialize]
        public Sidebar sidebar => sidebars[anchor];

        [DoNotSerialize]
        public bool enabled { get; private set; }

        [Serialize]
        private object controlHint;

        [Serialize]
        public float height { get; set; }

        [Serialize]
        public SidebarAnchor anchor { get; set; }

        [Serialize]
        private Vector2 scroll;

        [Serialize]
        public int order { get; set; }

        [DoNotSerialize]
        private EventWrapper e;

        [DoNotSerialize]
        private ISidebarPanelContent _content;

        [DoNotSerialize]
        public ISidebarPanelContent content
        {
            get => _content;
            private set
            {
                _content = value;
                controlHint = content?.sidebarControlHint;
                e = new EventWrapper(controlHint);
                enabled = content != null;
            }
        }

        [DoNotSerialize]
        private bool isResizing;

        public bool TryAssociate(ISidebarPanelContent content)
        {
            if (content.sidebarControlHint == controlHint)
            {
                this.content = content;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Disable()
        {
            _content = null;
            enabled = false;
        }

        public void OnGUI(Rect position, ref float y, float? heightOverride = null)
        {
            if (content == null)
            {
                var message = "Missing sidebar content.";
                var messageType = MessageType.Warning;
                var messageHeight = LudiqGUIUtility.GetHelpBoxHeight(message, messageType, position.width);
                EditorGUI.HelpBox(position.VerticalSection(ref y, messageHeight), message, messageType);
                return;
            }

            var height = heightOverride ?? this.height;

            var panelPosition = new Rect
                (
                position.x,
                y,
                position.width,
                height
                );

            var titlePosition = new Rect
                (
                position.x,
                y,
                position.width,
                Styles.title.fixedHeight
                );

            if (e.IsRepaint)
            {
                Styles.title.Draw(titlePosition, content.titleContent, false, false, false, false);
            }

            var orderSpinnerPosition = new Rect
                (
                position.xMax - Styles.orderSpinnerWidth,
                y,
                Styles.orderSpinnerWidth,
                titlePosition.height
                );

            sidebar.OrderSpinner(orderSpinnerPosition, this);

            var anchorButtonPosition = new Rect
                (
                orderSpinnerPosition.x - Styles.anchorButtonWidth,
                y,
                Styles.anchorButtonWidth,
                titlePosition.height
                );

            DrawAnchorButton(anchorButtonPosition);

            y += titlePosition.height;

            var panelContentPosition = new Rect
                (
                position.x,
                y,
                position.width,
                height - titlePosition.height
                );

            if (e.IsRepaint)
            {
                Styles.background.Draw(panelContentPosition, false, false, false, false);
            }

            LudiqGUIUtility.BeginScrollablePanel(panelContentPosition, content.GetHeight, out var panelInnerPosition, ref scroll, new RectOffset(1, 1, 0, 0));

            content.OnGUI(panelInnerPosition);

            LudiqGUIUtility.EndScrollablePanel();

            y += panelContentPosition.height;

            if (heightOverride == null)
            {
                HandleResize(panelPosition);
            }
        }

        private void DrawAnchorButton(Rect position)
        {
            SidebarAnchor destination;
            EditorTexture icon;

            switch (anchor)
            {
                case SidebarAnchor.Left:
                    destination = SidebarAnchor.Right;
                    icon = BoltCore.Icons.sidebarAnchorRight;
                    break;

                case SidebarAnchor.Right:
                    destination = SidebarAnchor.Left;
                    icon = BoltCore.Icons.sidebarAnchorLeft;
                    break;

                default:
                    throw new UnexpectedEnumValueException<SidebarAnchor>(anchor);
            }

            if (GUI.Button(position, icon?.Single(), LudiqStyles.toolbarButton))
            {
                anchor = destination;
            }
        }

        private void HandleResize(Rect panelPosition)
        {
            var resizeArea = new Rect
                (
                panelPosition.x,
                panelPosition.yMax - (Styles.resizeGrip / 2),
                panelPosition.width - Styles.orderSpinnerWidth,
                Styles.resizeGrip
                );

            EditorGUIUtility.AddCursorRect(resizeArea, MouseCursor.ResizeVertical);

            e.HandleCapture(resizeArea.Contains(e.mousePosition), false);

            if (e.IsMouseDown(MouseButton.Left) && resizeArea.Contains(e.mousePosition))
            {
                isResizing = true;
            }

            if (isResizing && e.IsMouseUp(MouseButton.Left))
            {
                isResizing = false;
            }

            if (isResizing && e.IsMouseDrag(MouseButton.Left))
            {
                height = e.mousePosition.y - panelPosition.y;
            }

            e.HandleRelease();
        }

        private static class Styles
        {
            public static readonly GUIStyle background;
            public static readonly GUIStyle title;
            public static readonly float resizeGrip = 14f;
            public static readonly float orderSpinnerWidth = LudiqGUIUtility.scrollBarWidth;
            public static readonly float anchorButtonWidth = 22;

            static Styles()
            {
                background = ColorPalette.unityBackgroundLight.CreateBackground();

                title = new GUIStyle(EditorStyles.toolbar);
                title.fontSize = 11;
                title.alignment = TextAnchor.MiddleLeft;
                title.padding = new RectOffset(4, 4, 0, 0);
                title.imagePosition = ImagePosition.ImageLeft;
                title.fixedHeight = 18;
            }
        }
    }
}
