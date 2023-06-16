using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Executes the output ports in order.
    /// </summary>
    [UnitCategory("Control")]
    [UnitOrder(13)]
    public sealed class Sequence : Unit
    {
        [SerializeAs(nameof(outputCount))]
        private int _outputCount = 2;

        /// <summary>
        /// The entry point for the sequence.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        [DoNotSerialize]
        [Inspectable, InspectorLabel("Steps"), UnitHeaderInspectable("Steps")]
        public int outputCount
        {
            get => _outputCount;
            set => _outputCount = Mathf.Clamp(value, 1, 10);
        }

        [DoNotSerialize]
        public ReadOnlyCollection<ControlOutput> multiOutputs { get; private set; }

        protected override void Definition()
        {
            enter = ControlInputCoroutine(nameof(enter), Enter, EnterCoroutine);

            var _multiOutputs = new List<ControlOutput>();

            multiOutputs = _multiOutputs.AsReadOnly();

            for (var i = 0; i < outputCount; i++)
            {
                var output = ControlOutput(i.ToString());

                Succession(enter, output);

                _multiOutputs.Add(output);
            }
        }

        private ControlOutput Enter(Flow flow)
        {
            var stack = flow.PreserveStack();

            foreach (var output in multiOutputs)
            {
                flow.Invoke(output);

                flow.RestoreStack(stack);
            }

            flow.DisposePreservedStack(stack);

            return null;
        }

        private IEnumerator EnterCoroutine(Flow flow)
        {
            var stack = flow.PreserveStack();

            foreach (var output in multiOutputs)
            {
                yield return output;

                flow.RestoreStack(stack);
            }

            flow.DisposePreservedStack(stack);
        }

        public void CopyFrom(Sequence source)
        {
            base.CopyFrom(source);
            outputCount = source.outputCount;
        }
    }
}
