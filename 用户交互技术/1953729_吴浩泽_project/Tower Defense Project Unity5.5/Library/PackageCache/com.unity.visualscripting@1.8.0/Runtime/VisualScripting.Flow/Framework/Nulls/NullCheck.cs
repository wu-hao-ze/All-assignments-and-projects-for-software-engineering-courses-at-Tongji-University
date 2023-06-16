using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Branches flow depending on whether the input is null.
    /// </summary>
    [UnitCategory("Nulls")]
    [TypeIcon(typeof(Null))]
    public sealed class NullCheck : Unit
    {
        /// <summary>
        /// The input.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput input { get; private set; }

        /// <summary>
        /// The entry point for the null check.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The action to execute if the input is not null.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Not Null")]
        public ControlOutput ifNotNull { get; private set; }

        /// <summary>
        /// The action to execute if the input is null.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Null")]
        public ControlOutput ifNull { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);
            input = ValueInput<object>(nameof(input)).AllowsNull();
            ifNotNull = ControlOutput(nameof(ifNotNull));
            ifNull = ControlOutput(nameof(ifNull));

            Requirement(input, enter);
            Succession(enter, ifNotNull);
            Succession(enter, ifNull);
        }

        public ControlOutput Enter(Flow flow)
        {
            var input = flow.GetValue(this.input);

            bool isNull;

            if (input is UnityObject)
            {
                // Required cast because of Unity's custom == operator.
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                isNull = (UnityObject)input == null;
            }
            else
            {
                isNull = input == null;
            }

            if (isNull)
            {
                return ifNull;
            }
            else
            {
                return ifNotNull;
            }
        }
    }
}
