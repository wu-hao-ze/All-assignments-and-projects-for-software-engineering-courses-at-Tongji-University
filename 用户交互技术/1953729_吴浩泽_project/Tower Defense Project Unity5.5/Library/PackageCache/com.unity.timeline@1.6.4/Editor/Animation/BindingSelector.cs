using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    class BindingSelector
    {
        TreeViewController m_TreeView;
        private BindingTreeViewGUI m_TreeViewGUI;
        public TreeViewController treeViewController
        {
            get { return m_TreeView; }
        }

        TreeViewState m_TrackGlobalTreeViewState;
        TreeViewState m_TreeViewState;
        BindingTreeViewDataSource m_TreeViewDataSource;
        CurveDataSource m_CurveDataSource;
        TimelineWindow m_Window;
        CurveEditor m_CurveEditor;
        ReorderableList m_DopeLines;

        string[] m_StringList = { };
        int[] m_Selection;
        bool m_PartOfSelection;
        public BindingSelector(EditorWindow window, CurveEditor curveEditor, TreeViewState trackGlobalTreeViewState)
        {
            m_Window = window as TimelineWindow;
            m_CurveEditor = curveEditor;
            m_TrackGlobalTreeViewState = trackGlobalTreeViewState;

            m_DopeLines = new ReorderableList(m_StringList, typeof(string), false, false, false, false);
            m_DopeLines.drawElementBackgroundCallback = null;
            m_DopeLines.showDefaultBackground = false;
            m_DopeLines.index = 0;
            m_DopeLines.headerHeight = 0;
            m_DopeLines.elementHeight = 20;
            m_DopeLines.draggable = false;
        }

        public void OnGUI(Rect targetRect)
        {
            if (m_TreeView == null)
                return;

            m_TreeView.OnEvent();
            m_TreeViewGUI.parentWidth = targetRect.width;
            m_TreeView.OnGUI(targetRect, GUIUtility.GetControlID(FocusType.Passive));
        }

        public void InitIfNeeded(Rect rect, CurveDataSource dataSource, bool isNewSelection)
        {
            if (Event.current.type != EventType.Layout)
                return;

            m_CurveDataSource = dataSource;
            var clip = dataSource.animationClip;

            List<EditorCurveBinding> allBindings = new List<EditorCurveBinding>();
            allBindings.Add(new EditorCurveBinding { propertyName = "Summary" });
            if (clip != null)
                allBindings.AddRange(AnimationUtility.GetCurveBindings(clip));

            m_DopeLines.list = allBindings.ToArray();

            if (m_TreeViewState != null)
            {
                if (isNewSelection)
                    RefreshAll();

                return;
            }

            m_TreeViewState = m_TrackGlobalTreeViewState != null ? m_TrackGlobalTreeViewState : new TreeViewState();

            m_TreeView = new TreeViewController(m_Window, m_TreeViewState)
            {
                useExpansionAnimation = false,
                deselectOnUnhandledMouseDown = true
            };

            m_TreeView.selectionChangedCallback += OnItemSelectionChanged;

            m_TreeViewDataSource = new BindingTreeViewDataSource(m_TreeView, clip, m_CurveDataSource);

            m_TreeViewGUI = new BindingTreeViewGUI(m_TreeView);
            m_TreeView.Init(rect, m_TreeViewDataSource, m_TreeViewGUI, null);

            m_TreeViewDataSource.UpdateData();

            RefreshSelection();
        }

        void OnItemSelectionChanged(int[] selection)
        {
            RefreshSelection(selection);
        }

        void RefreshAll()
        {
            RefreshTree();
            RefreshSelection();
        }

        void RefreshSelection()
        {
            RefreshSelection(m_TreeViewState.selectedIDs != null ? m_TreeViewState.selectedIDs.ToArray() : null);
        }

        void RefreshSelection(int[] selection)
        {
            if (selection == null || selection.Length == 0)
            {
                // select all.
                if (m_TreeViewDataSource.GetRows().Count > 0)
                {
                    m_Selection = m_TreeViewDataSource.GetRows().Select(r => r.id).ToArray();
                }
            }
            else
            {
                m_Selection = selection;
            }

            RefreshCurves();
        }

        public void RefreshCurves()
        {
            if (m_CurveDataSource == null || m_Selection == null)
                return;

            var bindings = new HashSet<EditorCurveBinding>(AnimationPreviewUtilities.EditorCurveBindingComparer.Instance);
            foreach (int s in m_Selection)
            {
                var item = (CurveTreeViewNode)m_TreeView.FindItem(s);
                if (item != null && item.bindings != null)
                    bindings.UnionWith(item.bindings);
            }
            var wrappers = m_CurveDataSource.GenerateWrappers(bindings);
            m_CurveEditor.animationCurves = wrappers.ToArray();
        }

        public void RefreshTree()
        {
            if (m_TreeViewDataSource == null)
                return;

            if (m_Selection == null)
                m_Selection = new int[0];

            // get the names of the previous items
            var selected = m_Selection.Select(x => m_TreeViewDataSource.FindItem(x)).Where(t => t != null).Select(c => c.displayName).ToArray();

            // update the source
            m_TreeViewDataSource.UpdateData();

            // find the same items
            var reselected = m_TreeViewDataSource.GetRows().Where(x => selected.Contains(x.displayName)).Select(x => x.id).ToArray();
            if (!reselected.Any())
            {
                if (m_TreeViewDataSource.GetRows().Count > 0)
                {
                    reselected = new[] { m_TreeViewDataSource.GetItem(0).id };
                }
            }

            // update the selection
            OnItemSelectionChanged(reselected);
        }

        internal virtual bool IsRenamingNodeAllowed(TreeViewItem node)
        {
            return false;
        }
    }
}
