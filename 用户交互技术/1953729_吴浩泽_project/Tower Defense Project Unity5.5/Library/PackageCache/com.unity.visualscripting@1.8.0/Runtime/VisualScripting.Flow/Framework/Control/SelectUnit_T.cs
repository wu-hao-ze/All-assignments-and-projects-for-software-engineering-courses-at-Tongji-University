using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [TypeIcon(typeof(ISelectUnit))]
    public abstract class SelectUnit<T> : Unit, ISelectUnit
    {
        // Using L<KVP> instead of Dictionary to allow null key
        [DoNotSerialize]
        public List<KeyValuePair<T, ValueInput>> branches { get; private set; }

        [Inspectable, Serialize]
        public List<T> options { get; set; } = new List<T>();

        /// <summary>
        /// The value on which to select.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput selector { get; private set; }

        /// <summary>
        /// The output value to return if the selector doesn't match any other option.
        /// </summary>
        [DoNotSerialize]
        public ValueInput @default { get; private set; }

        /// <summary>
        /// The selected value.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput selection { get; private set; }

        public override bool canDefine => options != null;

        protected override void Definition()
        {
            selection = ValueOutput(nameof(selection), Result).Predictable();

            selector = ValueInput<T>(nameof(selector));

            Requirement(selector, selection);

            branches = new List<KeyValuePair<T, ValueInput>>();

            foreach (var option in options)
            {
                var key = "%" + option;

                if (!valueInputs.Contains(key))
                {
                    var branch = ValueInput<object>(key).AllowsNull();
                    branches.Add(new KeyValuePair<T, ValueInput>(option, branch));
                    Requirement(branch, selection);
                }
            }

            @default = ValueInput<object>(nameof(@default));

            Requirement(@default, selection);
        }

        protected virtual bool Matches(T a, T b)
        {
            return Equals(a, b);
        }

        public object Result(Flow flow)
        {
            var selector = flow.GetValue<T>(this.selector);

            foreach (var branch in branches)
            {
                if (Matches(branch.Key, selector))
                {
                    return flow.GetValue(branch.Value);
                }
            }

            return flow.GetValue(@default);
        }
    }
}
