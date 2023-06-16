using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [TypeIcon(typeof(IBranchUnit))]
    public abstract class SwitchUnit<T> : Unit, IBranchUnit
    {
        // Using L<KVP> instead of Dictionary to allow null key
        [DoNotSerialize]
        public List<KeyValuePair<T, ControlOutput>> branches { get; private set; }

        [Inspectable, Serialize]
        public List<T> options { get; set; } = new List<T>();

        /// <summary>
        /// The entry point for the switch.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The value on which to switch.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput selector { get; private set; }

        /// <summary>
        /// The branch to take if the input value does not match any other option.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput @default { get; private set; }

        public override bool canDefine => options != null;

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);

            selector = ValueInput<T>(nameof(selector));

            Requirement(selector, enter);

            branches = new List<KeyValuePair<T, ControlOutput>>();

            foreach (var option in options)
            {
                var key = "%" + option;

                if (!controlOutputs.Contains(key))
                {
                    var branch = ControlOutput(key);
                    branches.Add(new KeyValuePair<T, ControlOutput>(option, branch));
                    Succession(enter, branch);
                }
            }

            @default = ControlOutput(nameof(@default));
            Succession(enter, @default);
        }

        protected virtual bool Matches(T a, T b)
        {
            return Equals(a, b);
        }

        public ControlOutput Enter(Flow flow)
        {
            var selector = flow.GetValue<T>(this.selector);

            foreach (var branch in branches)
            {
                if (Matches(branch.Key, selector))
                {
                    return branch.Value;
                }
            }

            return @default;
        }
    }
}
