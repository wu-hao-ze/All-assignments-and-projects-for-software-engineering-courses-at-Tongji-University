using UnityEngine;

namespace Unity.VisualScripting
{
    [UnitOrder(502)]
    public abstract class MoveTowards<T> : Unit
    {
        /// <summary>
        /// The current value.
        /// </summary>
        [DoNotSerialize]
        public ValueInput current { get; private set; }

        /// <summary>
        /// The target value.
        /// </summary>
        [DoNotSerialize]
        public ValueInput target { get; private set; }

        /// <summary>
        /// The maximum scalar increment between values.
        /// </summary>
        [DoNotSerialize]
        public ValueInput maxDelta { get; private set; }

        /// <summary>
        /// The incremented value.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput result { get; private set; }

        [Serialize, Inspectable, UnitHeaderInspectable("Per Second"), InspectorToggleLeft]
        public bool perSecond { get; set; }

        [DoNotSerialize]
        protected virtual T defaultCurrent => default(T);

        [DoNotSerialize]
        protected virtual T defaultTarget => default(T);

        protected override void Definition()
        {
            current = ValueInput(nameof(current), defaultCurrent);
            target = ValueInput(nameof(target), defaultTarget);
            maxDelta = ValueInput<float>(nameof(maxDelta), 0);
            result = ValueOutput(nameof(result), Operation);

            Requirement(current, result);
            Requirement(target, result);
            Requirement(maxDelta, result);
        }

        private T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(current), flow.GetValue<T>(target), flow.GetValue<float>(maxDelta) * (perSecond ? Time.deltaTime : 1));
        }

        public abstract T Operation(T current, T target, float maxDelta);
    }
}
