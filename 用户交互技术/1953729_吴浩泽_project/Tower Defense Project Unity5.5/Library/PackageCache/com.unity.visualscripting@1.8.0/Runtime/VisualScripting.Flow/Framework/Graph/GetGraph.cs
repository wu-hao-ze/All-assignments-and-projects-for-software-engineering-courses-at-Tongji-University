using UnityEngine;

namespace Unity.VisualScripting
{
    [UnitCategory("Graphs/Graph Nodes")]
    public abstract class GetGraph<TGraph, TGraphAsset, TMachine> : Unit
        where TGraph : class, IGraph, new()
        where TGraphAsset : Macro<TGraph>
        where TMachine : Machine<TGraph, TGraphAsset>
    {
        /// <summary>
        /// The GameObject to retrieve the graph from.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput gameObject { get; protected set; }

        /// <summary>
        /// The graph that is set on the GameObject.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Graph")]
        [PortLabelHidden]
        public ValueOutput graphOutput { get; protected set; }

        protected override void Definition()
        {
            gameObject = ValueInput<GameObject>(nameof(gameObject), null).NullMeansSelf();
            graphOutput = ValueOutput(nameof(graphOutput), Get);
        }

        TGraphAsset Get(Flow flow)
        {
            var go = flow.GetValue<GameObject>(gameObject);
            return go.GetComponent<TMachine>().nest.macro;
        }
    }
}
