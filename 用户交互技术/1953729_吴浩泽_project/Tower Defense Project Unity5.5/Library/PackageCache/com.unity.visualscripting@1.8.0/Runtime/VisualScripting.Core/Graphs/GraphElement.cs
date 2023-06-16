using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.VisualScripting
{
    public abstract class GraphElement<TGraph> : IGraphElement where TGraph : class, IGraph
    {
        [Serialize]
        public Guid guid { get; set; } = Guid.NewGuid();

        // To minimize the amount of implementation needed, and simplify inversion of control,
        // we provide instantiation routines that fit most types of elements in the right order and
        // we provide implemented defaults for interfaces that the element could implement.
        // Normally, an element shouldn't have to override instantiate or uninstantiate directly.

        public virtual void Instantiate(GraphReference instance)
        {
            // Create the data for this graph, non-recursively, that is
            // required for the nest instantiation, so we do it first.
            if (this is IGraphElementWithData withData)
            {
                instance.data.CreateElementData(withData);
            }

            // Nest instantiation is a recursive operation that will
            // call Instantiate on descendant graphs and elements.
            // Because event listening will descend graph data recursively,
            // we need to create it before.
            if (this is IGraphNesterElement nester && nester.nest.graph != null)
            {
                GraphInstances.Instantiate(instance.ChildReference(nester, true));
            }

            // StartListening is a recursive operation that will
            // descend graphs recursively. It must be called after
            // instantiation of all child graphs because it will require
            // their data. The drawback with this approach is that it will be
            // called at each step bubbling up, whereas it will only
            // effectively trigger once we reach the top level.
            // Traversal is currently O(n!) where n is the number of descendants,
            // ideally it would be O(n) if only triggered from the root.

            // => Listening has to be implemented by Bolt classes, because Graphs isn't aware of the event system
        }

        public virtual void Uninstantiate(GraphReference instance)
        {
            // See above comments, in reverse order.

            if (this is IGraphNesterElement nester && nester.nest.graph != null)
            {
                GraphInstances.Uninstantiate(instance.ChildReference(nester, true));
            }

            if (this is IGraphElementWithData withData)
            {
                instance.data.FreeElementData(withData);
            }
        }

        public virtual void BeforeAdd() { }

        public virtual void AfterAdd()
        {
            var instances = GraphInstances.OfPooled(graph);

            foreach (var instance in instances)
            {
                Instantiate(instance);
            }

            instances.Free();
        }

        public virtual void BeforeRemove()
        {
            var instances = GraphInstances.OfPooled(graph);

            foreach (var instance in instances)
            {
                Uninstantiate(instance);
            }

            instances.Free();

            Dispose();
        }

        public virtual void AfterRemove() { }

        public virtual void Dispose() { }

        protected void InstantiateNest()
        {
            var nester = (IGraphNesterElement)this;

            if (graph == null)
            {
                return;
            }

            var instances = GraphInstances.OfPooled(graph);

            foreach (var instance in instances)
            {
                GraphInstances.Instantiate(instance.ChildReference(nester, true));
            }

            instances.Free();
        }

        protected void UninstantiateNest()
        {
            var nester = (IGraphNesterElement)this;

            var instances = GraphInstances.ChildrenOfPooled(nester);

            foreach (var instance in instances)
            {
                GraphInstances.Uninstantiate(instance);
            }

            instances.Free();
        }

        #region Graph

        [DoNotSerialize]
        public virtual int dependencyOrder => 0;

        public virtual bool HandleDependencies() => true;

        [DoNotSerialize]
        public TGraph graph { get; set; }

        [DoNotSerialize]
        IGraph IGraphElement.graph
        {
            get => graph;
            set
            {
                Ensure.That(nameof(value)).IsOfType<TGraph>(value);
                graph = (TGraph)value;
            }
        }

        [DoNotSerialize]
        IGraph IGraphItem.graph => graph;

        #endregion

        #region Poutine

        public virtual IEnumerable<ISerializationDependency> deserializationDependencies => Enumerable.Empty<ISerializationDependency>();

        public virtual IEnumerable<object> GetAotStubs(HashSet<object> visited)
        {
            return Enumerable.Empty<object>();
        }

        public virtual void Prewarm() { }

        protected void CopyFrom(GraphElement<TGraph> source) { }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(GetType().Name);
            sb.Append("#");
            sb.Append(guid.ToString().Substring(0, 5));
            sb.Append("...");
            return sb.ToString();
        }

        #endregion

        public virtual AnalyticsIdentifier GetAnalyticsIdentifier()
        {
            throw new NotImplementedException();
        }
    }
}
