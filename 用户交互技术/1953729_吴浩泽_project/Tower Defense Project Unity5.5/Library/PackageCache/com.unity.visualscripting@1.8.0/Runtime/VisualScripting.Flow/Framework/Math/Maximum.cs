using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    [UnitOrder(302)]
    public abstract class Maximum<T> : MultiInputUnit<T>
    {
        /// <summary>
        /// The maximum.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput maximum { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            maximum = ValueOutput(nameof(maximum), Operation).Predictable();

            foreach (var multiInput in multiInputs)
            {
                Requirement(multiInput, maximum);
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
