using JetBrains.Annotations;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Set a StateGraphAsset to a StateMachine
    /// </summary>
    [TypeIcon(typeof(StateGraph))]
    public class SetStateGraph : SetGraph<StateGraph, StateGraphAsset, StateMachine>
    {
        /// <summary>
        /// The type of object that handles the graph.
        /// </summary>
        [Serialize, Inspectable, UnitHeaderInspectable, UsedImplicitly]
        public StateGraphContainerType containerType { get; set; }

        protected override bool isGameObject => containerType == StateGraphContainerType.GameObject;
    }
}
