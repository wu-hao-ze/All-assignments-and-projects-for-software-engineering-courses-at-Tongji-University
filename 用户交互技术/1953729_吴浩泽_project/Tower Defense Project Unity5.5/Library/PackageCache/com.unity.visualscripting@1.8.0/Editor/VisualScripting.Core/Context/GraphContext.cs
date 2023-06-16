using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public abstract class GraphContext<TGraph, TCanvas> : IGraphContext
        where TGraph : class, IGraph
        where TCanvas : class, ICanvas
    {
        protected GraphContext(GraphReference reference)
        {
            Ensure.That(nameof(reference)).IsNotNull(reference);
            Ensure.That(nameof(reference.graph)).IsOfType<TGraph>(reference.graph);

            this.reference = reference;

            analyserProvider = new AnalyserProvider(reference);
            graphMetadata = Metadata.Root().StaticObject(reference.graph);
            extensions = this.Extensions().ToList().AsReadOnly();
            sidebarPanels = SidebarPanels().ToList().AsReadOnly();
        }

        public GraphReference reference { get; }

        public AnalyserProvider analyserProvider { get; }

        public TCanvas canvas => (TCanvas)graph.Canvas();

        ICanvas IGraphContext.canvas => canvas;

        public GraphSelection selection => canvas.selection;

        public TGraph graph => (TGraph)reference.graph;

        IGraph IGraphContext.graph => graph;

        public Metadata graphMetadata { get; }

        public Metadata selectionMetadata => selection.Count == 1 ? ElementMetadata(selection.Single()) : null;

        public Metadata ElementMetadata(IGraphElement element)
        {
            // Static object is faster than indexer
            // return graphMetadata[nameof(IGraph.elements)].Indexer(element.guid).Cast(element.GetType());
            return graphMetadata[nameof(IGraph.elements)].StaticObject(element);
        }

        public ReadOnlyCollection<IGraphContextExtension> extensions { get; }

        IEnumerable<IGraphContextExtension> IGraphContext.extensions => extensions;

        public virtual string windowTitle => "Graph";

        protected virtual IEnumerable<ISidebarPanelContent> SidebarPanels() => Enumerable.Empty<ISidebarPanelContent>();

        public ReadOnlyCollection<ISidebarPanelContent> sidebarPanels { get; }

        IEnumerable<ISidebarPanelContent> IGraphContext.sidebarPanels => sidebarPanels;

        public bool isPrefabInstance => reference.serializedObject?.IsConnectedPrefabInstance() ?? false;

        #region Lifecycle

        public virtual void Dispose()
        {
            // Manually free the providers so that the
            // disposable decorators free their event handlers

            analyserProvider?.FreeAll();
        }

        public void BeginEdit(bool disablePrefabInstance = true)
        {
            LudiqEditorUtility.editedObject.BeginOverride(reference.serializedObject);
            LudiqGraphsEditorUtility.editedContext.BeginOverride(this);
            EditorGUI.BeginDisabledGroup(disablePrefabInstance && isPrefabInstance);
            EditorGUI.BeginChangeCheck();
        }

        public void EndEdit()
        {
            if (EditorGUI.EndChangeCheck())
            {
                DescribeAndAnalyze();
            }

            EditorGUI.EndDisabledGroup();
            LudiqGraphsEditorUtility.editedContext.EndOverride();
            LudiqEditorUtility.editedObject.EndOverride();
        }

        public void DescribeAndAnalyze()
        {
            foreach (var element in graph.elements)
            {
                if (element.HasDescriptor())
                {
                    element.Describe();
                }
            }

            graph.Describe();

            analyserProvider.AnalyzeAll();

            reference.parent.Describe();
        }

        #endregion
    }
}
