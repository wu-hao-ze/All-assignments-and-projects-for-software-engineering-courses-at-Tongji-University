using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    [UnitOrder(301)]
    public abstract class Minimum<T> : MultiInputUnit<T>
    {
        /// <summary>
        /// The minimum.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput minimum { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            minimum = ValueOutput(nameof(minimum), Operation).Predictable();

            foreach (var multiInput in multiInputs)
            {
                Requirement(multiInput, minimum);
            }
        }

        public abstract T Operation(T a, T b);
        public abstract T Operation(IEnumerable<T> values);

        public T Operation(Flow flow)
        {
            if (inputCount == 2)
            {
                return Operation(flow.GetValue<T>(multiInputs[0]), flow.GetValue<T>(multiInputs[1]));
            }
            else
            {
                return Operation(multiInputs.Select(flow.GetValue<T>));
            }
        }
    }
}
