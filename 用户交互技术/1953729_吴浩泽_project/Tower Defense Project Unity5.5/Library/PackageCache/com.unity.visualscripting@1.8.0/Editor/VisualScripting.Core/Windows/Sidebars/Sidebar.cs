using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class Sidebar
    {
        public Sidebar(Sidebars sidebars, SidebarAnchor anchor)
        {
            this.sidebars = sidebars;
            this.anchor = anchor;

            e = new EventWrapper(this);
        }

        [DoNotSerialize]
        private Sidebars sidebars { get; }

        [DoNotSerialize]
        private SidebarAnchor anchor { get; }

        [DoNotSerialize]
        private EventWrapper e { get; }

        [DoNotSerialize]
        public bool show => displayedPanels.Count > 0;

        [DoNotSerialize]
        private bool isResizing;

        [Serialize]
        private float size { get; set; } = 300;

        [Serialize]
        private Vector2 scroll;

        [DoNotSerialize]
        public readonly List<SidebarPanel> displayedPanels = new List<SidebarPanel>();

        public void DrawLayout()
        {
            CacheDisplayedPanels();

            if (!show)
            {
                return;
            }

            var position = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(GetWidth()), GUILayout.ExpandHeight(true));

            if (!e.IsLayout && !Event.current.ShouldSkip())
            {
                OnGUI(position);
            }
        }

        public void Remove<T>() where T : ISidebarPanelContent
        {
            foreach (var panel in displayedPanels.Where(p => p.content is T))
            {
                panel.Disable();
            }
        }

        private void CacheDisplayedPanels()
        {
            displayedPanels.Clear();
            displayedPanels.AddRange(sidebars.panels.Where(p => p.enabled && p.anchor == anchor).OrderBy(p => p.order));
        }

        public float GetWidth()
        {
            foreach (var panel in displayedPanels)
            {
                size = Mathf.Max(size, panel.content.minSize.x);
            }

            return size;
        }

        private float GetHeight(float totalHeight)
        {
            var height = 0f;

            for (int i = 0; i < displayedPanels.Count; i++)
            {
                var panel = displayedPanels[i];
                var isLast = i == displayedPanels.Count - 1;

                var minPanelHeight = panel.content.minSize.y;
                panel.height = Mathf.Max(panel.height, minPanelHeight);

                var panelHeight = panel.height;

                if (isLast && height < totalHeight)
                {
                    var remainingHeight = totalHeight - height - 1;
                    panelHeight = Mathf.Max(remainingHeight, minPanelHeight);
                }

                height += panelHeight;

                height++; // Separator
            }

            return height;
        }

        public void OnGUI(Rect position)
        {
            HandleResizing(position);

            if (Event.current.ShouldSkip(position))
            {
                return;
            }

            if (e.IsRepaint)
            {
                Styles.background.Draw(position, false, false, false, false);
            }

            if (anchor == SidebarAnchor.Left)
            {
                // leave space for scrolling after the potential scrollbar
                position.width -= Styles.resizeGrip;
            }

            LudiqGUIUtility.BeginScrollablePanel(position, width => GetHeight(position.height), out Rect sidebarScrolledPosition, ref scroll);

            var y = sidebarScrolledPosition.y;

            for (int i = 0; i < displayedPanels.Count; i++)
            {
                var panel = displayedPanels[i];
                var isLast = i == displayedPanels.Count - 1;

                if (isLast)
                {
                    var remainingHeight = sidebarScrolledPosition.height - y - 1;
                    var minPanelHeight = panel.content.minSize.y;
                    var heightOverride = Mathf.Max(remainingHeight, minPanelHeight);
                    panel.OnGUI(sidebarScrolledPosition, ref y, heightOverride);
                }
                else
                {
                    panel.OnGUI(sidebarScrolledPosition, ref y);
                }

                if (e.IsRepaint)
                {
                    Styles.separator.Draw(sidebarScrolledPosition.VerticalSection(ref y, 1), false, false, false, false);
                }
            }

            LudiqGUIUtility.EndScrollablePanel();

            if (e.IsRepaint)
            {
                Styles.separator.Draw(new Rect(position.x, position.y, 1, position.height), false, false, false, false);
            }
        }

        private void HandleResizing(Rect position)
        {
            Rect resizeArea;

            switch (anchor)
            {
                case SidebarAnchor.Left:
                    resizeArea = new Rect
                        (
                        position.xMax - Styles.resizeGrip,
                        position.y,
                        Styles.resizeGrip,
                        position.height
                        );
                    break;

                case SidebarAnchor.Right:
                    resizeArea = new Rect
                        (
                        position.x,
                        position.y,
                        Styles.resizeGrip,
                        position.height
                        );
                    break;

                default:
                    throw new UnexpectedEnumValueException<SidebarAnchor>(anchor);
            }

            EditorGUIUtility.AddCursorRect(resizeArea, MouseCursor.SplitResizeLeftRight);

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
                switch (anchor)
                {
                    case SidebarAnchor.Left:
                        size = e.mousePosition.x - position.xMin;
                        break;

                    case SidebarAnchor.Right:
                        size = position.xMax - e.mousePosition.x;
                        break;

                    default:
                        throw new UnexpectedEnumValueException<SidebarAnchor>(anchor);
                }
            }

            e.HandleRelease();
        }

        public void OrderSpinner(Rect position, SidebarPanel panel)
        {
            Ensure.That(nameof(panel)).IsNotNull(panel);

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Arrow);

            var isFirst = displayedPanels.FirstOrDefault() == panel;
            var isLast = displayedPanels.LastOrDefault() == panel;

            var orderIncrement = -LudiqGUI.Spinner(position, !isFirst, !isLast);

            if (orderIncrement == 0)
            {
                return;
            }

            if (orderIncrement == +1)
            {
                foreach (var otherPanel in displayedPanels)
                {
                    if (otherPanel != panel && otherPanel.order >= panel.order)
                    {
                        panel.order = otherPanel.order;
                        otherPanel.order--;
                        break;
                    }
                }
            }
            else if (orderIncrement == -1)
            {
                foreach (var otherPanel in displayedPanels)
                {
                    if (otherPanel != panel && otherPanel.order <= panel.order)
                    {
                        panel.order = otherPanel.order;
                        otherPanel.order++;
                        break;
                    }
                }
            }

            CacheDisplayedPanels();
        }

        private static class Styles
        {
            public static readonly GUIStyle background;
            public static readonly GUIStyle separator;
            public static readonly float resizeGrip = 2f;

            static Styles()
            {
                background = ColorPalette.unityBackgroundMid.CreateBackground();
                separator = ColorPalette.unityBackgroundDark.CreateBackground();
            }
        }
    }
}
