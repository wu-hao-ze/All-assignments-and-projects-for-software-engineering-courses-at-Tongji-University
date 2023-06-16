using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Delays flow by waiting until multiple input flows have been executed.
    /// </summary>
    [UnitCategory("Time")]
    [UnitOrder(6)]
    [TypeIcon(typeof(WaitUnit))]
    public sealed class WaitForFlow : Unit, IGraphElementWithData
    {
        public sealed class Data : IGraphElementData
        {
            public bool[] inputsActivated;
            public bool isWaitingCoroutine;
        }

        /// <summary>
        /// Whether the activation status should be reset on exit.
        /// </summary>
        [Serialize]
        [Inspectable]
        public bool resetOnExit { get; set; }

        [SerializeAs(nameof(inputCount))]
        private int _inputCount = 2;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Inputs")]
        public int inputCount
        {
            get => _inputCount;
            set => _inputCount = Mathf.Clamp(value, 2, 10);
        }

        [DoNotSerialize]
        public ReadOnlyCollection<ControlInput> awaitedInputs { get; private set; }

        /// <summary>
        /// Trigger to reset the activation status.
        /// </summary>
        [DoNotSerialize]
        public ControlInput reset { get; private set; }

        /// <summary>
        /// Triggered after all inputs have been entered at least once.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            var _awaitedInputs = new List<ControlInput>();

            awaitedInputs = _awaitedInputs.AsReadOnly();

            exit = ControlOutput(nameof(exit));

            for (var i = 0; i < inputCount; i++)
            {
                var _i = i; // Cache outside closure

                var awaitedInput = ControlInputCoroutine(_i.ToString(), (flow) => Enter(flow, _i), (flow) => EnterCoroutine(flow, _i));

                _awaitedInputs.Add(awaitedInput);

                Succession(awaitedInput, exit);
            }

            reset = ControlInput(nameof(reset), Reset);
        }

        public IGraphElementData CreateData()
        {
            return new Data() { inputsActivated = new bool[inputCount] };
        }

        private ControlOutput Enter(Flow flow, int index)
        {
            var data = flow.stack.GetElementData<Data>(this);

            data.inputsActivated[index] = true;

            if (CheckActivated(flow))
            {
                if (resetOnExit)
                {
                    Reset(flow);
                }

                return exit;
            }
            else
            {
                return null;
            }
        }

        private bool CheckActivated(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            for (int i = 0; i < data.inputsActivated.Length; i++)
            {
                if (!data.inputsActivated[i])
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerator EnterCoroutine(Flow flow, int index)
        {
            var data = flow.stack.GetElementData<Data>(this);

            data.inputsActivated[index] = true;

            if (data.isWaitingCoroutine)
            {
                // Another input started an async wait,
                // we'll let that flow be responsible for
                // triggering the exit.
                yield break;
            }

            if (!CheckActivated(flow))
            {
                data.isWaitingCoroutine = true;

                yield return new WaitUntil(() => CheckActivated(flow));

                data.isWaitingCoroutine = false;
            }

            if (resetOnExit)
            {
                Reset(flow);
            }

            yield return exit;
        }

        private ControlOutput Reset(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            for (int i = 0; i < data.inputsActivated.Length; i++)
            {
                data.inputsActivated[i] = false;
            }

            return null;
        }
    }
}
