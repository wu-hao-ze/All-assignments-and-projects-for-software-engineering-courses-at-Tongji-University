using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public abstract class Machine<TGraph, TMacro> : LudiqBehaviour, IMachine
        where TGraph : class, IGraph, new()
        where TMacro : Macro<TGraph>
    {
        protected Machine()
        {
            nest.nester = this;
            nest.source = GraphSource.Macro;
        }

        [Serialize]
        public GraphNest<TGraph, TMacro> nest { get; private set; } = new GraphNest<TGraph, TMacro>();

        [DoNotSerialize]
        IGraphNest IGraphNester.nest => nest;

        // We use our own alive and enabled bools which are thread safe.
        // Alive is true in Awake and OnEnable, and becomes false in OnDestroy.
        // Enabled is not true during Awake, to avoid double OnEnable triggering due to GraphNest delegates
        // See: https://support.ludiq.io/communities/5/topics/1971-a/
        [DoNotSerialize]
        private bool _alive;

        [DoNotSerialize]
        private bool _enabled;

        [DoNotSerialize]
        private GameObject threadSafeGameObject;

        [DoNotSerialize]
        GameObject IMachine.threadSafeGameObject => threadSafeGameObject;

        [DoNotSerialize]
        private bool isReferenceCached;

        [DoNotSerialize]
        private GraphReference _reference;

        [DoNotSerialize]
        protected GraphReference reference => isReferenceCached ? _reference : GraphReference.New(this, false);

        [DoNotSerialize]
        protected bool hasGraph => reference != null;

        [DoNotSerialize]
        public TGraph graph => nest.graph;

        [DoNotSerialize]
        public IGraphData graphData { get; set; }

        [DoNotSerialize]
        bool IGraphParent.isSerializationRoot => true;

        [DoNotSerialize]
        UnityObject IGraphParent.serializedObject
        {
            get
            {
                switch (nest.source)
                {
                    case GraphSource.Macro: return nest.macro;
                    case GraphSource.Embed: return this;
                    default: throw new UnexpectedEnumValueException<GraphSource>(nest.source);
                }
            }
        }

        [DoNotSerialize]
        IGraph IGraphParent.childGraph => graph;

        public IEnumerable<object> GetAotStubs(HashSet<object> visited)
        {
            return nest.GetAotStubs(visited);
        }

        public bool isDescriptionValid
        {
            get => true;
            set { }
        }

        protected virtual void Awake()
        {
            _alive = true;
            threadSafeGameObject = gameObject;

            nest.afterGraphChange += CacheReference;
            nest.beforeGraphChange += ClearCachedReference;

            CacheReference();

            if (graph != null)
            {
                graph.Prewarm();
                InstantiateNest();
            }
        }

        protected virtual void OnEnable()
        {
            _enabled = true;
        }

        protected virtual void OnInstantiateWhileEnabled()
        {
        }

        protected virtual void OnUninstantiateWhileEnabled()
        {
        }

        protected virtual void OnDisable()
        {
            _enabled = false;
        }

        protected virtual void OnDestroy()
        {
            ClearCachedReference();

            if (graph != null)
            {
                UninstantiateNest();
            }

            threadSafeGameObject = null;
            _alive = false;
        }

        protected virtual void OnValidate()
        {
            threadSafeGameObject = gameObject;
        }

        public GraphPointer GetReference()
        {
            return reference;
        }

        private void CacheReference()
        {
            _reference = GraphReference.New(this, false);
            isReferenceCached = true;
        }

        private void ClearCachedReference()
        {
            _reference = null;
        }

        public virtual void InstantiateNest()
        {
            if (_alive)
            {
                GraphInstances.Instantiate(reference);
            }

            if (_enabled)
            {
                if (UnityThread.allowsAPI)
                {
                    OnInstantiateWhileEnabled();
                }
                else
                {
                    Debug.LogWarning($"Could not run instantiation events on {this.ToSafeString()} because the Unity API is not available.\nThis can happen when undoing / redoing a graph source change.", this);
                }
            }
        }

        public virtual void UninstantiateNest()
        {
            if (_enabled)
            {
                if (UnityThread.allowsAPI)
                {
                    OnUninstantiateWhileEnabled();
                }
                else
                {
                    Debug.LogWarning($"Could not run uninstantiation events on {this.ToSafeString()} because the Unity API is not available.\nThis can happen when undoing / redoing a graph source change.", this);
                }
            }

            if (_alive)
            {
                var instances = GraphInstances.ChildrenOfPooled(this);

                foreach (var instance in instances)
                {
                    GraphInstances.Uninstantiate(instance);
                }

                instances.Free();
            }
        }

#if MODULE_ANIMATION_EXISTS
        // Should be in EventMachine logically, but kept here for legacy compatibility.
        public virtual void TriggerAnimationEvent(AnimationEvent animationEvent)
        {
        }
#endif

        // Should be in EventMachine logically, but kept here for legacy compatibility.
        public virtual void TriggerUnityEvent(string name)
        {
        }

        public abstract TGraph DefaultGraph();

        IGraph IGraphParent.DefaultGraph() => DefaultGraph();
    }
}
