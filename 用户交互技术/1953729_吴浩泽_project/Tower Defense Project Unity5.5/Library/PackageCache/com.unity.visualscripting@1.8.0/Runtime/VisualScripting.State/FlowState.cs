using System.ComponentModel;
using UnityEngine;

namespace Unity.VisualScripting
{
    [SerializationVersion("A")]
    [TypeIcon(typeof(FlowGraph))]
    [DisplayName("Script State")]
    public sealed class FlowState : NesterState<FlowGraph, ScriptGraphAsset>, IGraphEventListener
    {
        public FlowState() { }

        public FlowState(ScriptGraphAsset macro) : base(macro) { }


        #region Lifecycle

        protected override void OnEnterImplementation(Flow flow)
        {
            if (flow.stack.TryEnterParentElement(this))
            {
                nest.graph.StartListening(flow.stack);
                flow.stack.TriggerEventHandler(hook => hook == StateEventHooks.OnEnterState, new EmptyEventArgs(), parent => parent is SubgraphUnit, false);
                flow.stack.ExitParentElement();
            }
        }

        protected override void OnExitImplementation(Flow flow)
        {
            if (flow.stack.TryEnterParentElement(this))
            {
                flow.stack.TriggerEventHandler(hook => hook == StateEventHooks.OnExitState, new EmptyEventArgs(), parent => parent is SubgraphUnit, false);
                nest.graph.StopListening(flow.stack);
                flow.stack.ExitParentElement();
            }
        }

        public void StartListening(GraphStack stack)
        {
            if (stack.TryEnterParentElement(this))
            {
                nest.graph.StartListening(stack);
                stack.ExitParentElement();
            }
        }

        public void StopListening(GraphStack stack)
        {
            if (stack.TryEnterParentElement(this))
            {
                nest.graph.StopListening(stack);
                stack.ExitParentElement();
            }
        }

        public bool IsListening(GraphPointer pointer)
        {
            return pointer.GetElementData<Data>(this).isActive;
        }

        #endregion


        #region Factory

        public override FlowGraph DefaultGraph()
        {
            return GraphWithEnterUpdateExit();
        }

        public static FlowState WithEnterUpdateExit()
        {
            var flowState = new FlowState();
            flowState.nest.source = GraphSource.Embed;
            flowState.nest.embed = GraphWithEnterUpdateExit();
            return flowState;
        }

        public static FlowGraph GraphWithEnterUpdateExit()
        {
            return new FlowGraph
            {
                units =
                {
                    new OnEnterState { position = new Vector2(-205, -215) },
                    new Update { position = new Vector2(-161, -38) },
                    new OnExitState { position = new Vector2(-205, 145) }
                }
            };
        }

        #endregion
    }
}
