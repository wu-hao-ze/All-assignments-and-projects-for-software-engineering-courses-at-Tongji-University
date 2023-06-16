using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Merges two or more lists together.
    /// </summary>
    [UnitCategory("Collections/Lists")]
    [UnitOrder(7)]
    public sealed class MergeLists : MultiInputUnit<IEnumerable>
    {
        /// <summary>
        /// The merged list.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput list { get; private set; }

        protected override void Definition()
        {
            list = ValueOutput(nameof(list), Merge);

            base.Definition();

            foreach (var input in multiInputs)
            {
                Requirement(input, list);
            }
        }

        public IList Merge(Flow flow)
        {
            var list = new AotList();

            for (var i = 0; i < inputCount; i++)
            {
                list.AddRange(flow.GetValue<IEnumerable>(multiInputs[i]));
            }

            return list;
        }
    }
}
