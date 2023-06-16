using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class State : GraphElement<StateGraph>, IState
    {
        public class Data : IGraphElementData
        {
            public bool isActive;

            public bool hasEntered;
        }

        public class DebugData : IStateDebugData
        {
            public int lastEnterFrame { get; set; }

            public float lastExitTime { get; set; }

            public Exception runtimeException { get; set; }
        }

        public IGraphElementData CreateData()
        {
            return new Data();
        }

        public IGraphElementDebugData CreateDebugData()
        {
            return new DebugData();
        }

        [Serialize]
        public bool isStart { get; set; }

        [DoNotSerialize]
        public virtual bool canBeSource => true;

        [DoNotSerialize]
        public virtual bool canBeDestination => true;

        public override void BeforeRemove()
        {
            base.BeforeRemove();

            Disconnect();
        }

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);

            var data = instance.GetElementData<Data>(this);

            if (this is IGraphEventListener listener && data.isActive)
            {
                listener.StartListening(instance);
            }
            else if (isStart && !data.hasEntered && graph.IsListening(instance))
            {
                using (var flow = Flow.New(instance))
                {
                    OnEnter(flow, StateEnterReason.Start);
                }
            }
        }

        public override void Uninstantiate(GraphReference instance)
        {
            if (this is IGraphEventListener listener)
            {
                listener.StopListening(instance);
            }

            base.Uninstantiate(instance);
        }

        #region Poutine

        protected void CopyFrom(State source)
        {
            base.CopyFrom(source);

            isStart = source.isStart;
            width = source.width;
        }

        #endregion

        #region Transitions

        public IEnumerable<IStateTransition> outgoingTransitions => graph?.transitions.WithSource(this) ?? Enumerable.Empty<IStateTransition>();

        public IEnumerable<IStateTransition> incomingTransitions => graph?.transitions.WithDestination(this) ?? Enumerable.Empty<IStateTransition>();

        protected List<IStateTransition> outgoingTransitionsNoAlloc => graph?.transitions.WithSourceNoAlloc(this) ?? Empty<IStateTransition>.list;

        public IEnumerable<IStateTransition> transitions => LinqUtility.Concat<IStateTransition>(outgoingTransitions, incomingTransitions);

        public void Disconnect()
        {
            foreach (var transition in transitions.ToArray())
            {
                graph.transitions.Remove(transition);
            }
        }

        #endregion

        #region Lifecycle

        public virtual void OnEnter(Flow flow, StateEnterReason reason)
        {
            var data = flow.stack.GetElementData<Data>(this);

            if (data.isActive) // Prevent re-entry from Any State
            {
                return;
            }

            data.isActive = true;

            data.hasEntered = true;

            foreach (var transition in outgoingTransitionsNoAlloc)
            {
                // Start listening for the transition's events
                // before entering the state in case OnEnterState
                // actually instantly triggers a transition via event
                // http://support.ludiq.io/topics/261-event-timing-issue/
                (transition as IGraphEventListener)?.StartListening(flow.stack);
            }

            if (flow.enableDebug)
            {
                var editorData = flow.stack.GetElementDebugData<DebugData>(this);

                editorData.lastEnterFrame = EditorTimeBinding.frame;
            }

            OnEnterImplementation(flow);

            foreach (var transition in outgoingTransitionsNoAlloc)
            {
                try
                {
                    transition.OnEnter(flow);
                }
                catch (Exception ex)
                {
                    transition.HandleException(flow.stack, ex);
                    throw;
                }
            }
        }

        public virtual void OnExit(Flow flow, StateExitReason reason)
        {
            var data = flow.stack.GetElementData<Data>(this);

            if (!data.isActive)
            {
                return;
            }

            OnExitImplementation(flow);

            data.isActive = false;

            if (flow.enableDebug)
            {
                var editorData = flow.stack.GetElementDebugData<DebugData>(this);

                editorData.lastExitTime = EditorTimeBinding.time;
            }

            foreach (var transition in outgoingTransitionsNoAlloc)
            {
                try
                {
                    transition.OnExit(flow);
                }
                catch (Exception ex)
                {
                    transition.HandleException(flow.stack, ex);
                    throw;
                }
            }
        }

        protected virtual void OnEnterImplementation(Flow flow) { }

        protected virtual void UpdateImplementation(Flow flow) { }

        protected virtual void FixedUpdateImplementation(Flow flow) { }

        protected virtual void LateUpdateImplementation(Flow flow) { }

        protected virtual void OnExitImplementation(Flow flow) { }

        public virtual void OnBranchTo(Flow flow, IState destination) { }

        #endregion

        #region Widget

        public const float DefaultWidth = 170;

        [Serialize]
        public Vector2 position { get; set; }

        [Serialize]
        public float width { get; set; } = DefaultWidth;

        #endregion

        #region Analytics

        public override AnalyticsIdentifier GetAnalyticsIdentifier()
        {
            var aid = new AnalyticsIdentifier
            {
                Identifier = GetType().FullName,
                Namespace = GetType().Namespace,
            };
            aid.Hashcode = aid.Identifier.GetHashCode();
            return aid;
        }

        #endregion
    }
}
