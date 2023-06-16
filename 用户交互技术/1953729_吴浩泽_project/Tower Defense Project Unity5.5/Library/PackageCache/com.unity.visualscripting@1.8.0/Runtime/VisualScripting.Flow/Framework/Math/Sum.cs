using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    [UnitOrder(303)]
    [TypeIcon(typeof(Add<>))]
    public abstract class Sum<T> : MultiInputUnit<T>
    {
        /// <summary>
        /// The sum.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput sum { get; private set; }

        protected override void Definition()
        {
            if (this is IDefaultValue<T> defaultValueUnit)
            {
                var mi = new List<ValueInput>();
                multiInputs = mi.AsReadOnly();

                for (var i = 0; i < inputCount; i++)
                {
                    if (i == 0)
                    {
                        mi.Add(ValueInput<T>(i.ToString()));
                        continue;
                    }

                    mi.Add(ValueInput(i.ToString(), defaultValueUnit.defaultValue));
                }
            }
            else
            {
                base.Definition();
            }

            sum = ValueOutput(nameof(sum), Operation).Predictable();

            foreach (var multiInput in multiInputs)
            {
                Requirement(multiInput, sum);
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
