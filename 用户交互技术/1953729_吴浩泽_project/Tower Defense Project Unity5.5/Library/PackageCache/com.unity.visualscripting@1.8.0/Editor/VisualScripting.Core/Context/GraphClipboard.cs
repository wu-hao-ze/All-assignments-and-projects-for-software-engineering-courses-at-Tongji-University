using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    [InitializeAfterPlugins]
    public static class GraphClipboard
    {
        static GraphClipboard()
        {
            singleClipboard = new Clipboard();
            groupClipboard = new Clipboard();

            GraphWindow.activeContextChanged += OnContextChange;
        }

        private static Event e => Event.current;

        private static void OnContextChange(IGraphContext context)
        {
            GraphClipboard.context = context;
        }

        private static IGraphContext context;

        #region Context Shortcuts

        private static GraphReference reference => context.reference;

        private static IGraph graph => context.graph;

        private static ICanvas canvas => context.canvas;

        private static GraphSelection selection => context.selection;

        #endregion

        public static Clipboard singleClipboard { get; }

        public static Clipboard groupClipboard { get; }

        public static bool canCopySelection => selection.Count > 0;

        public static bool canPaste
        {
            get
            {
                if (selection.Count == 1 && CanPasteInside(selection.Single()))
                {
                    return true;
                }
                else
                {
                    return canPasteOutside;
                }
            }
        }

        public static bool canPasteOutside => groupClipboard.containsData && GetPasteGroup().Any();

        public static bool canDuplicateSelection => GetCopyGroup(selection).Count > 0;

        private static HashSet<IGraphElement> GetCopyGroup(IEnumerable<IGraphElement> elements)
        {
            var copyGroup = new HashSet<IGraphElement>();

            foreach (var element in elements)
            {
                copyGroup.Add(element);

                canvas.Widget(element).ExpandCopyGroup(copyGroup);
            }

            canvas.ShrinkCopyGroup(copyGroup);

            return copyGroup;
        }

        private static List<IGraphElement> GetPasteGroup()
        {
            return groupClipboard.Paste<HashSet<IGraphElement>>()
                .Where(e => graph.elements.Includes(e.GetType()))
                .OrderBy(e => e.dependencyOrder)
                .ToList();
        }

        public static void CopyElement(IGraphElement element)
        {
            Ensure.That(nameof(element)).IsNotNull(element);

            singleClipboard.Copy(element);
            groupClipboard.Copy(GetCopyGroup(element.Yield()));
        }

        public static void CopySelection()
        {
            if (!canCopySelection)
            {
                throw new InvalidOperationException();
            }

            if (selection.Count == 1)
            {
                CopyElement(selection.Single());
            }
            else
            {
                singleClipboard.Clear();
                groupClipboard.Copy(GetCopyGroup(selection));
            }

            e?.TryUse();
        }

        public static void Paste(Vector2? position = null)
        {
            if (!canPaste)
            {
                throw new InvalidOperationException();
            }

            if (selection.Count == 1 && CanPasteInside(selection.Single()))
            {
                PasteInside(selection.Single());
            }
            else
            {
                PasteOutside(true, position);
            }
        }

        public static bool CanPasteInside(IGraphElement element)
        {
            Ensure.That(nameof(element)).IsNotNull(element);

            // TODO: A solid PasteInside implementation would work like ReplaceUnit:
            // Implement an IPreservable interface, preserve, remove, recreate, apply.
            // This would make entirely sure that all OnAdd/OnRemove handlers get called,
            // and wouldn't require any per-element implementation. Plus, it would
            // allow pasting across element types while preserving connections/transitions!

            return false;
        }

        public static void PasteInside(IGraphElement element)
        {
            Ensure.That(nameof(element)).IsNotNull(element);

            if (!CanPasteInside(element))
            {
                throw new InvalidOperationException();
            }

            UndoUtility.RecordEditedObject("Paste Graph Element");

            throw new NotImplementedException();

            //GUI.changed = true;
            //e?.TryUse();
        }

        public static void PasteOutside(bool reposition, Vector2? position = null)
        {
            if (!canPasteOutside)
            {
                throw new InvalidOperationException();
            }

            UndoUtility.RecordEditedObject("Paste Graph Elements");

            var pastedElements = GetPasteGroup();

            // Assign new GUIDs

            foreach (var pastedElement in pastedElements)
            {
                pastedElement.guid = Guid.NewGuid();
            }

            // Add elements to graph and selection

            selection.Clear();

            foreach (var pastedElement in pastedElements)
            {
                if (!pastedElement.HandleDependencies())
                {
                    continue;
                }

                graph.elements.Add(pastedElement);

                selection.Add(pastedElement);
            }

            canvas.Cache();

            foreach (var pastedElement in pastedElements)
            {
                var pastedWidget = canvas.Widget(pastedElement);
                pastedWidget.BringToFront();
            }

            var pastedWidgets = pastedElements.Select(e => canvas.Widget(e)).ToList();

            // Recenter elements in graph view

            if (reposition)
            {
                var area = GraphGUI.CalculateArea(pastedWidgets.Where(widget => widget.canDrag));

                Vector2 delta;

                if (position.HasValue)
                {
                    delta = position.Value - area.position;
                }
                else
                {
                    delta = graph.pan - area.center;
                }

                foreach (var pastedWidget in pastedWidgets)
                {
                    if (pastedWidget.canDrag)
                    {
                        pastedWidget.position = new Rect(pastedWidget.position.position + delta, pastedWidget.position.size).PixelPerfect();
                        pastedWidget.Reposition();
                        pastedWidget.CachePositionFirstPass();
                        pastedWidget.CachePosition();
                    }
                }
            }

            // Space out overlapping elements

            foreach (var pastedWidget in pastedWidgets)
            {
                if (pastedWidget.canDrag)
                {
                    var distanciation = 20;
                    var timeout = 100;
                    var timeoutIndex = 0;

                    while (GraphGUI.PositionOverlaps(canvas, pastedWidget, 5))
                    {
                        // Space the widget out
                        pastedWidget.position = new Rect(pastedWidget.position.position + new Vector2(distanciation, distanciation), pastedWidget.position.size).PixelPerfect();

                        // Calculate the resulting position immediately
                        pastedWidget.CachePositionFirstPass();
                        pastedWidget.CachePosition();

                        // Mark it as invalid still so dependencies like ports will be recached
                        pastedWidget.Reposition();

                        // Failsafe to keep the editor from freezing
                        if (++timeoutIndex > timeout)
                        {
                            Debug.LogWarning($"Failed to space out pasted element: {pastedWidget.element}");
                            break;
                        }
                    }
                }
            }

            canvas.Cache();

            GUI.changed = true;

            e?.TryUse();
        }

        public static void CutSelection()
        {
            UndoUtility.RecordEditedObject("Cut Graph Element Selection");
            CopySelection();
            canvas.DeleteSelection();
        }

        public static void DuplicateSelection()
        {
            object singleClipboardRestore = null;
            object groupClipboardRestore = null;

            if (singleClipboard.containsData)
            {
                singleClipboardRestore = singleClipboard.Paste();
            }

            if (groupClipboard.containsData)
            {
                groupClipboardRestore = groupClipboard.Paste();
            }

            UndoUtility.RecordEditedObject("Duplicate Graph Element Selection");
            CopySelection();
            PasteOutside(false);

            if (singleClipboardRestore != null)
            {
                singleClipboard.Copy(singleClipboardRestore);
            }
            else
            {
                singleClipboard.Clear();
            }

            if (groupClipboardRestore != null)
            {
                groupClipboard.Copy(groupClipboardRestore);
            }
            else
            {
                groupClipboard.Clear();
            }
        }
    }
}
