using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    [SerializationVersion("A")]
    public sealed class StateGraph : Graph, IGraphEventListener
    {
        public StateGraph()
        {
            states = new GraphElementCollection<IState>(this);
            transitions = new GraphConnectionCollection<IStateTransition, IState, IState>(this);
            groups = new GraphElementCollection<GraphGroup>(this);
            sticky = new GraphElementCollection<StickyNote>(this);

            elements.Include(states);
            elements.Include(transitions);
            elements.Include(groups);
            elements.Include(sticky);
        }

        public override IGraphData CreateData()
        {
            return new StateGraphData(this);
        }

        public void StartListening(GraphStack stack)
        {
            stack.GetGraphData<StateGraphData>().isListening = true;

            var activeStates = GetActiveStatesNoAlloc(stack);

            foreach (var state in activeStates)
            {
                (state as IGraphEventListener)?.StartListening(stack);
            }

            activeStates.Free();
        }

        public void StopListening(GraphStack stack)
        {
            var activeStates = GetActiveStatesNoAlloc(stack);

            foreach (var state in activeStates)
            {
                (state as IGraphEventListener)?.StopListening(stack);
            }

            activeStates.Free();

            stack.GetGraphData<StateGraphData>().isListening = false;
        }

        public bool IsListening(GraphPointer pointer)
        {
            return pointer.GetGraphData<StateGraphData>().isListening;
        }

        #region Elements

        [DoNotSerialize]
        public GraphElementCollection<IState> states { get; internal set; }

        [DoNotSerialize]
        public GraphConnectionCollection<IStateTransition, IState, IState> transitions { get; internal set; }

        [DoNotSerialize]
        public GraphElementCollection<GraphGroup> groups { get; internal set; }

        [DoNotSerialize]
        public GraphElementCollection<StickyNote> sticky { get; private set; }
        #endregion


        #region Lifecycle

        // Active state detection happens twice:
        //
        // 1. Before the enumeration, because any state
        //    that becomes active during an update shouldn't
        //    be updated until the next update
        //
        // 2. Inside the update method, because a state
        //    that was active during enumeration and no longer
        //    is shouldn't be updated.

        private HashSet<IState> GetActiveStatesNoAlloc(GraphPointer pointer)
        {
            var activeStates = HashSetPool<IState>.New();

            foreach (var state in states)
            {
                var stateData = pointer.GetElementData<State.Data>(state);

                if (stateData.isActive)
                {
                    activeStates.Add(state);
                }
            }

            return activeStates;
        }

        public void Start(Flow flow)
        {
            flow.stack.GetGraphData<StateGraphData>().isListening = true;

            foreach (var state in states.Where(s => s.isStart))
            {
                try
                {
                    state.OnEnter(flow, StateEnterReason.Start);
                }
                catch (Exception ex)
                {
                    state.HandleException(flow.stack, ex);
                    throw;
                }
            }
        }

        public void Stop(Flow flow)
        {
            var activeStates = GetActiveStatesNoAlloc(flow.stack);

            foreach (var state in activeStates)
            {
                try
                {
                    state.OnExit(flow, StateExitReason.Stop);
                }
                catch (Exception ex)
                {
                    state.HandleException(flow.stack, ex);
                    throw;
                }
            }

            activeStates.Free();

            flow.stack.GetGraphData<StateGraphData>().isListening = false;
        }

        #endregion


        public static StateGraph WithStart()
        {
            var stateGraph = new StateGraph();

            var startState = FlowState.WithEnterUpdateExit();
            startState.isStart = true;
            startState.nest.embed.title = "Start";
            startState.position = new Vector2(-86, -15);

            stateGraph.states.Add(startState);

            return stateGraph;
        }
    }
}
