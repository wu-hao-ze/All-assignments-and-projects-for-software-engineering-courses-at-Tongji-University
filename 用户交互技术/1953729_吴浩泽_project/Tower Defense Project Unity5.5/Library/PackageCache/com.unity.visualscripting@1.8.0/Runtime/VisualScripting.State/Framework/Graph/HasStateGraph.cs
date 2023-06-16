using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Check if a GameObject or StateMachine has a StateGraph
    /// </summary>
    [TypeIcon(typeof(StateGraph))]
    [UnitCategory("Graphs/Graph Nodes")]
    public sealed class HasStateGraph : HasGraph<StateGraph, StateGraphAsset, StateMachine>
    {
        /// <summary>
        /// The type of object that handles the graph.
        /// </summary>
        [Serialize, Inspectable, UnitHeaderInspectable, UsedImplicitly]
        public StateGraphContainerType containerType { get; set; }

        protected override bool isGameObject => containerType == StateGraphContainerType.GameObject;
    }
}
