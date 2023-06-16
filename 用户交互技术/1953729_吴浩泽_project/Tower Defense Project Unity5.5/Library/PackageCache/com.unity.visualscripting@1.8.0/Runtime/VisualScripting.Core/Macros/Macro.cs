using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [DisableAnnotation]
    public abstract class Macro<TGraph> : MacroScriptableObject, IMacro
        where TGraph : class, IGraph, new()
    {
        [SerializeAs(nameof(graph))]
        private TGraph _graph = new TGraph();

        [DoNotSerialize]
        public TGraph graph
        {
            get => _graph;
            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("Macros must have a graph.");
                }

                if (value == graph)
                {
                    return;
                }

                _graph = value;
            }
        }

        [DoNotSerialize]
        IGraph IMacro.graph
        {
            get => graph;
            set => graph = (TGraph)value;
        }

        [DoNotSerialize]
        IGraph IGraphParent.childGraph => graph;

        public IEnumerable<object> GetAotStubs(HashSet<object> visited)
        {
            return graph.GetAotStubs(visited);
        }

        [DoNotSerialize]
        bool IGraphParent.isSerializationRoot => true;

        [DoNotSerialize]
        UnityObject IGraphParent.serializedObject => this;

        [DoNotSerialize]
        private GraphReference _reference = null;

        [DoNotSerialize]
        protected GraphReference reference => _reference == null ? GraphReference.New(this, false) : _reference;

        public bool isDescriptionValid
        {
            get => true;
            set { }
        }

        protected override void OnBeforeDeserialize()
        {
            base.OnBeforeDeserialize();

            Serialization.NotifyDependencyDeserializing(this);
        }

        protected override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            Serialization.NotifyDependencyDeserialized(this);
        }

        public abstract TGraph DefaultGraph();

        IGraph IGraphParent.DefaultGraph()
        {
            return DefaultGraph();
        }

        // This seems to fix the legendary undo bug!
        // https://support.ludiq.io/communities/5/topics/4434-undo-bug-isolated
        // The issue seems to be that newly created assets don't receive OnAfterDeserialize,
        // and therefore never notify the dependencies system that they became available.
        // Therefore, if any graph relied on a macro dependency (super unit, super state, flow state, state unit)
        // that was created before a deserialization of that dependency (usually enter/exit play mode, restart Unity),
        // it would silently never load, not throwing any error or warning along the way.
        // For example, creating a new flow macro, dragging it to create a super node in another graph,
        // then undoing, would corrupt the parent graph.
        // Note: this *could* go in Awake, but OnEnable seems to be more reliable and consistent. Awake
        // doesn't get called in play mode entry for example (but that doesn't matter because OnAfterDeserialize does anyway).
        protected virtual void OnEnable()
        {
            Serialization.NotifyDependencyAvailable(this);
        }

        // ScriptableObjects actually call OnDisable not OnDestroy when unloaded ("goes out of scope"),
        // so we need to unregister the dependency here.
        // https://forum.unity.com/threads/scriptableobject-behaviour-discussion-how-scriptable-objects-work.541212/
        // The doc also guarantees it will be called before OnDestroy, so no need to repeat that in OnDestory.
        protected virtual void OnDisable()
        {
            Serialization.NotifyDependencyUnavailable(this);
        }

        public GraphPointer GetReference()
        {
            return reference;
        }

        bool ISerializationDependency.IsDeserialized { get; set; }
    }
}
