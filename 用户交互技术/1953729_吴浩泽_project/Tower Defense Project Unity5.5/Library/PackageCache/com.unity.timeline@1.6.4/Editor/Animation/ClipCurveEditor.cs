using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor
{
    class ClipCurveEditor
    {
        static readonly GUIContent s_RemoveCurveContent = new GUIContent(L10n.Tr("Remove Curve"));
        static readonly GUIContent s_RemoveCurvesContent = new GUIContent(L10n.Tr("Remove Curves"));

        internal readonly CurveEditor m_CurveEditor;
        static readonly CurveEditorSettings s_CurveEditorSettings = new CurveEditorSettings
        {
            hSlider = false,
            vSlider = false,
            hRangeLocked = false,
            vRangeLocked = false,
            scaleWithWindow = true,
            hRangeMin = 0.0f,
            showAxisLabels = true,
            allowDeleteLastKeyInCurve = true,
            rectangleToolFlags = CurveEditorSettings.RectangleToolFlags.MiniRectangleTool
        };

        static readonly float s_GridLabelWidth = 40.0f;

        readonly BindingSelector m_BindingHierarchy;
        public BindingSelector bindingHierarchy
        {
            get { return m_BindingHierarchy; }
        }

        public Rect shownAreaInsideMargins
        {
            get { return m_CurveEditor != null ? m_CurveEditor.shownAreaInsideMargins : new Rect(1, 1, 1, 1); }
        }

        Vector2 m_ScrollPosition = Vector2.zero;

        readonly CurveDataSource m_DataSource;

        float m_LastFrameRate = 30.0f;

        UInt64 m_LastClipVersion = UInt64.MaxValue;

        TrackViewModelData m_ViewModel;
        bool m_ShouldRestoreShownArea;

        bool isNewSelection
        {
            get
            {
                if (m_ViewModel == null || m_DataSource == null)
                    return true;

                return m_ViewModel.lastInlineCurveDataID != m_DataSource.id;
            }
        }

        internal CurveEditor curveEditor
        {
            get { return m_CurveEditor; }
        }

        public ClipCurveEditor(CurveDataSource dataSource, TimelineWindow parentWindow, TrackAsset hostTrack)
        {
            m_DataSource = dataSource;

            m_CurveEditor = new CurveEditor(new Rect(0, 0, 1000, 100), new CurveWrapper[0], false);

            s_CurveEditorSettings.vTickStyle = new TickStyle
            {
                tickColor = { color = DirectorStyles.Instance.customSkin.colorInlineCurveVerticalLines },
                distLabel = 20,
                stubs = true
            };

            s_CurveEditorSettings.hTickStyle = new TickStyle
            {
                // hide horizontal lines by giving them a transparent color
                tickColor = { color = new Color(0.0f, 0.0f, 0.0f, 0.0f) },
                distLabel = 0
            };

            m_CurveEditor.settings = s_CurveEditorSettings;

            m_ViewModel = TimelineWindowViewPrefs.GetTrackViewModelData(hostTrack);

            m_ShouldRestoreShownArea = true;
            m_CurveEditor.ignoreScrollWheelUntilClicked = true;
            m_CurveEditor.curvesUpdated = OnCurvesUpdated;

            m_BindingHierarchy = new BindingSelector(parentWindow, m_CurveEditor, m_ViewModel.inlineCurvesState);
        }

        public void SelectAllKeys()
        {
            m_CurveEditor.SelectAll();
        }

        public void FrameClip()
        {
            m_CurveEditor.InvalidateBounds();
            m_CurveEditor.FrameClip(false, true);
        }

        public CurveDataSource dataSource
        {
            get { return m_DataSource; }
        }

        // called when curves are edited
        internal void OnCurvesUpdated()
        {
            if (m_DataSource == null)
                return;

            if (m_CurveEditor == null)
                return;

            if (m_CurveEditor.animationCurves.Length == 0)
                return;

            List<CurveWrapper> curvesToUpdate = m_CurveEditor.animationCurves.Where(c => c.changed).ToList();

            // nothing changed, return.
            if (curvesToUpdate.Count == 0)
                return;

            // something changed, manage the undo properly.
            m_DataSource.ApplyCurveChanges(curvesToUpdate);
            m_LastClipVersion = m_DataSource.GetClipVersion();
        }

        public void DrawHeader(Rect headerRect)
        {
            m_BindingHierarchy.InitIfNeeded(headerRect, m_DataSource, isNewSelection);

            try
            {
                GUILayout.BeginArea(headerRect);
                m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
                m_BindingHierarchy.OnGUI(new Rect(0, 0, headerRect.width, headerRect.height));
                if (m_BindingHierarchy.treeViewController != null)
                    m_BindingHierarchy.treeViewController.contextClickItemCallback = ContextClickItemCallback;
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void ContextClickItemCallback(int obj)
        {
            GenerateContextMenu(obj);
        }

        void GenerateContextMenu(int obj = -1)
        {
            if (Event.current.type != EventType.ContextClick)
                return;

            var selectedCurves = GetSelectedProperties().ToArray();
            if (selectedCurves.Length > 0)
            {
                var menu = new GenericMenu();
                var content = selectedCurves.Length == 1 ? s_RemoveCurveContent : s_RemoveCurvesContent;
                menu.AddItem(content,
                    false,
                    () => RemoveCurves(selectedCurves)
                );
                menu.ShowAsContext();
            }
        }

        public IEnumerable<EditorCurveBinding> GetSelectedProperties(bool useForcedGroups = false)
        {
            var bindings = new HashSet<EditorCurveBinding>();
            var bindingTree = m_BindingHierarchy.treeViewController.data as BindingTreeViewDataSource;
            foreach (var selectedId in m_BindingHierarchy.treeViewController.GetSelection())
            {
                var node = bindingTree.FindItem(selectedId) as CurveTreeViewNode;
                if (node == null)
                    continue;

                var curveNodeParent = node.parent as CurveTreeViewNode;
                if (useForcedGroups && node.forceGroup && curveNodeParent != null)
                    bindings.UnionWith(curveNodeParent.bindings);
                else
                    bindings.UnionWith(node.bindings);
            }
            return bindings;
        }

        public void RemoveCurves(IEnumerable<EditorCurveBinding> bindings)
        {
            m_DataSource.RemoveCurves(bindings);
            m_BindingHierarchy.RefreshTree();
            TimelineWindow.instance.state.CalculateRowRects();
            m_LastClipVersion = m_DataSource.GetClipVersion();
        }

        class CurveEditorState : ICurveEditorState
        {
            public TimeArea.TimeFormat timeFormat { get; set; }
            public Vector2 timeRange => new Vector2(0, 1);
            public bool rippleTime => false;
        }

        void UpdateCurveEditorIfNeeded(WindowState state)
        {
            if ((Event.current.type != EventType.Layout) || (m_DataSource == null) || (m_BindingHierarchy == null))
                return;

            // check if the curves have changed externally
            var curveChange = m_DataSource.UpdateExternalChanges(ref m_LastClipVersion);
            if (curveChange == CurveChangeType.None)
                return;

            if (curveChange == CurveChangeType.CurveAddedOrRemoved)
                m_BindingHierarchy.RefreshTree();
            else // curve modified
                m_BindingHierarchy.RefreshCurves();

            m_CurveEditor.InvalidateSelectionBounds();

            m_CurveEditor.state = new CurveEditorState() { timeFormat = state.timeFormat.ToTimeAreaFormat() };
            m_CurveEditor.invSnap = (float)state.referenceSequence.frameRate;
        }

        public void DrawCurveEditor(Rect rect, WindowState state, Vector2 clipRange, bool loop, bool selected)
        {
            SetupMarginsAndRect(rect, state);
            UpdateCurveEditorIfNeeded(state);

            if (m_ShouldRestoreShownArea)
                RestoreShownArea();

            var curveVisibleTimeRange = CalculateCurveVisibleTimeRange(state.timeAreaShownRange, m_DataSource);
            m_CurveEditor.SetShownHRangeInsideMargins(curveVisibleTimeRange.x, curveVisibleTimeRange.y); //align the curve with the clip.

            if (m_LastFrameRate != state.referenceSequence.frameRate)
            {
                m_CurveEditor.hTicks.SetTickModulosForFrameRate((float)state.referenceSequence.frameRate);
                m_LastFrameRate = (float)state.referenceSequence.frameRate;
            }

            foreach (var cw in m_CurveEditor.animationCurves)
                cw.renderer.SetWrap(WrapMode.Default, loop ? WrapMode.Loop : WrapMode.Default);

            using (new GUIGroupScope(rect))
            {
                var localRect = new Rect(0.0f, 0.0f, rect.width, rect.height);
                var localClipRange = new Vector2(Mathf.Floor(clipRange.x - rect.xMin), Mathf.Ceil(clipRange.y - rect.xMin));
                var curveStartPosX = Mathf.Floor(state.TimeToPixel(m_DataSource.start) - rect.xMin);

                EditorGUI.DrawRect(new Rect(curveStartPosX, 0.0f, 1.0f, rect.height), new Color(1.0f, 1.0f, 1.0f, 0.5f));
                DrawCurveEditorBackground(localRect);

                if (selected)
                {
                    var selectionRect = new Rect(localClipRange.x, 0.0f, localClipRange.y - localClipRange.x, localRect.height);
                    DrawOutline(selectionRect);
                }

                EditorGUI.BeginChangeCheck();
                {
                    var evt = Event.current;
                    if (evt.type == EventType.Layout || evt.type == EventType.Repaint || selected)
                        m_CurveEditor.CurveGUI();
                }
                if (EditorGUI.EndChangeCheck())
                    OnCurvesUpdated();

                DrawOverlay(localRect, localClipRange, DirectorStyles.Instance.customSkin.colorInlineCurveOutOfRangeOverlay);
                DrawGrid(localRect, curveStartPosX);
            }
        }

        static Vector2 CalculateCurveVisibleTimeRange(Vector2 timeAreaShownRange, CurveDataSource curve)
        {
            var curveVisibleTimeRange = new Vector2
            {
                x = Math.Max(0.0f, timeAreaShownRange.x - curve.start),
                y = timeAreaShownRange.y - curve.start
            };
            return curveVisibleTimeRange * curve.timeScale;
        }

        void SetupMarginsAndRect(Rect rect, WindowState state)
        {
            var startX = state.TimeToPixel(m_DataSource.start) - rect.x;
            var timelineWidth = state.timeAreaRect.width;
            m_CurveEditor.rect = new Rect(0.0f, 0.0f, timelineWidth, rect.height);
            m_CurveEditor.leftmargin = Math.Max(startX, 0.0f);
            m_CurveEditor.rightmargin = 0.0f;
            m_CurveEditor.topmargin = m_CurveEditor.bottommargin = CalculateTopMargin(rect.height);
        }

        void RestoreShownArea()
        {
            if (isNewSelection)
                FrameClip();
            else
                m_CurveEditor.shownAreaInsideMargins = m_ViewModel.inlineCurvesShownAreaInsideMargins;
            m_ShouldRestoreShownArea = false;
        }

        static void DrawCurveEditorBackground(Rect rect)
        {
            if (EditorGUIUtility.isProSkin)
                return;

            var animEditorBackgroundRect = Rect.MinMaxRect(0.0f, rect.yMin, rect.xMax, rect.yMax);

            // Curves are not legible in Personal Skin so we need to darken the background a bit.
            EditorGUI.DrawRect(animEditorBackgroundRect, DirectorStyles.Instance.customSkin.colorInlineCurvesBackground);
        }

        static float CalculateTopMargin(float height)
        {
            return Mathf.Clamp(0.15f * height, 10.0f, 40.0f);
        }

        static void DrawOutline(Rect rect, float thickness = 2.0f)
        {
            // Draw top selected lines.
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Color.white);

            // Draw bottom selected lines.
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Color.white);

            // Draw Left Selected Lines
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Color.white);

            // Draw Right Selected Lines
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Color.white);
        }

        static void DrawOverlay(Rect rect, Vector2 clipRange, Color color)
        {
            var leftSide = new Rect(rect.xMin, rect.yMin, clipRange.x - rect.xMin, rect.height);
            EditorGUI.DrawRect(leftSide, color);

            var rightSide = new Rect(Mathf.Max(0.0f, clipRange.y), rect.yMin, rect.xMax, rect.height);
            EditorGUI.DrawRect(rightSide, color);
        }

        void DrawGrid(Rect rect, float curveXPosition)
        {
            var gridXPos = Mathf.Max(curveXPosition - s_GridLabelWidth, rect.xMin);
            var gridRect = new Rect(gridXPos, rect.y, s_GridLabelWidth, rect.height);
            var originalRect = m_CurveEditor.rect;

            m_CurveEditor.rect = new Rect(0.0f, 0.0f, rect.width, rect.height);
            using (new GUIGroupScope(gridRect))
                m_CurveEditor.GridGUI();
            m_CurveEditor.rect = originalRect;
        }
    }
}
