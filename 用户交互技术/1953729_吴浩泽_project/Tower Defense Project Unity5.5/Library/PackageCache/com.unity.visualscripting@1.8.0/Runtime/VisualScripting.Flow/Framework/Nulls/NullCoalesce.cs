using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Provides a fallback value if the input value is null.
    /// </summary>
    [UnitCategory("Nulls")]
    [TypeIcon(typeof(Null))]
    public sealed class NullCoalesce : Unit
    {
        /// <summary>
        /// The value.
        /// </summary>
        [DoNotSerialize]
        public ValueInput input { get; private set; }

        /// <summary>
        /// The fallback to use if the value is null.
        /// </summary>
        [DoNotSerialize]
        public ValueInput fallback { get; private set; }

        /// <summary>
        /// The returned value.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput result { get; private set; }

        protected override void Definition()
        {
            input = ValueInput<object>(nameof(input)).AllowsNull();
            fallback = ValueInput<object>(nameof(fallback));
            result = ValueOutput(nameof(result), Coalesce).Predictable();

            Requirement(input, result);
            Requirement(fallback, result);
        }

        public object Coalesce(Flow flow)
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

            return isNull ? flow.GetValue(fallback) : input;
        }
    }
}
