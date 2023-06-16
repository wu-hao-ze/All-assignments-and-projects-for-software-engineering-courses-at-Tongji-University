using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Creates a list from a number of item inputs.
    /// </summary>
    [UnitCategory("Collections/Lists")]
    [UnitOrder(-1)]
    [TypeIcon(typeof(IList))]
    public sealed class CreateList : MultiInputUnit<object>
    {
        [DoNotSerialize]
        protected override int minInputCount => 0;

        [InspectorLabel("Elements")]
        [UnitHeaderInspectable("Elements")]
        [Inspectable]
        public override int inputCount
        {
            get => base.inputCount;
            set => base.inputCount = value;
        }

        /// <summary>
        /// The created list.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput list { get; private set; }

        protected override void Definition()
        {
            list = ValueOutput(nameof(list), Create);

            base.Definition();

            foreach (var input in multiInputs)
            {
                Requirement(input, list);
            }

            InputsAllowNull();
        }

        public IList Create(Flow flow)
        {
            var list = new AotList();

            for (var i = 0; i < inputCount; i++)
            {
                list.Add(flow.GetValue<object>(multiInputs[i]));
            }

            return list;
        }
    }
}
