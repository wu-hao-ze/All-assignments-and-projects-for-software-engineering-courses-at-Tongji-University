using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    [UnitCategory("Graphs/Graph Nodes")]
    public abstract class GetGraphs<TGraph, TGraphAsset, TMachine> : Unit
        where TGraph : class, IGraph, new()
        where TGraphAsset : Macro<TGraph>
        where TMachine : Machine<TGraph, TGraphAsset>
    {
        /// <summary>
        /// The GameObject to retrieve the graphs from.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput gameObject { get; protected set; }

        /// <summary>
        /// The graph that is set on the GameObject.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Graphs")]
        [PortLabelHidden]
        public ValueOutput graphList { get; protected set; }

        protected override void Definition()
        {
            gameObject = ValueInput<GameObject>(nameof(gameObject), null).NullMeansSelf();
            graphList = ValueOutput(nameof(graphList), Get);
        }

        List<TGraphAsset> Get(Flow flow)
        {
            var go = flow.GetValue<GameObject>(gameObject);
            return go.GetComponents<TMachine>()
                .Where(machine => go.GetComponent<TMachine>().nest.macro != null)
                .Select(machine => machine.nest.macro)
                .ToList();
        }
    }
}
