using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public abstract class NesterStateTransition<TGraph, TMacro> : StateTransition, INesterStateTransition
        where TGraph : class, IGraph, new()
        where TMacro : Macro<TGraph>
    {
        protected NesterStateTransition()
        {
            nest.nester = this;
        }

        protected NesterStateTransition(IState source, IState destination) : base(source, destination)
        {
            nest.nester = this;
        }

        [Serialize]
        public GraphNest<TGraph, TMacro> nest { get; private set; } = new GraphNest<TGraph, TMacro>();

        [DoNotSerialize]
        IGraphNest IGraphNester.nest => nest;

        [DoNotSerialize]
        IGraph IGraphParent.childGraph => nest.graph;

        [DoNotSerialize]
        bool IGraphParent.isSerializationRoot => nest.source == GraphSource.Macro;

        [DoNotSerialize]
        UnityObject IGraphParent.serializedObject => nest.macro;

        [DoNotSerialize]
        public override IEnumerable<ISerializationDependency> deserializationDependencies => nest.deserializationDependencies;

        public override IEnumerable<object> GetAotStubs(HashSet<object> visited)
        {
            return LinqUtility.Concat<object>(base.GetAotStubs(visited), nest.GetAotStubs(visited));
        }

        protected void CopyFrom(NesterStateTransition<TGraph, TMacro> source)
        {
            base.CopyFrom(source);

            nest = source.nest;
        }

        public abstract TGraph DefaultGraph();

        IGraph IGraphParent.DefaultGraph() => DefaultGraph();

        void IGraphNester.InstantiateNest() => InstantiateNest();

        void IGraphNester.UninstantiateNest() => UninstantiateNest();
    }
}
