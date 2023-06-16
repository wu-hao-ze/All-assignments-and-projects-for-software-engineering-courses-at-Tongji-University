using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IState : IGraphElementWithDebugData, IGraphElementWithData
    {
        new StateGraph graph { get; }

        bool isStart { get; set; }

        bool canBeSource { get; }

        bool canBeDestination { get; }

        void OnBranchTo(Flow flow, IState destination);

        IEnumerable<IStateTransition> outgoingTransitions { get; }

        IEnumerable<IStateTransition> incomingTransitions { get; }

        IEnumerable<IStateTransition> transitions { get; }

        void OnEnter(Flow flow, StateEnterReason reason);

        void OnExit(Flow flow, StateExitReason reason);

        #region Widget

        Vector2 position { get; set; }

        float width { get; set; }

        #endregion
    }
}
