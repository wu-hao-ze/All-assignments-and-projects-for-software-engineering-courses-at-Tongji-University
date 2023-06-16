using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Checks whether a list contains the specified item.
    /// </summary>
    [UnitCategory("Collections/Lists")]
    [UnitSurtitle("List")]
    [UnitShortTitle("Contains Item")]
    [TypeIcon(typeof(IList))]
    public sealed class ListContainsItem : Unit
    {
        /// <summary>
        /// The list.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput list { get; private set; }

        /// <summary>
        /// The item.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput item { get; private set; }

        /// <summary>
        /// Whether the list contains the item.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput contains { get; private set; }

        protected override void Definition()
        {
            list = ValueInput<IList>(nameof(list));
            item = ValueInput<object>(nameof(item));
            contains = ValueOutput(nameof(contains), Contains);

            Requirement(list, contains);
            Requirement(item, contains);
        }

        public bool Contains(Flow flow)
        {
            var list = flow.GetValue<IList>(this.list);
            var item = flow.GetValue<object>(this.item);

            return list.Contains(item);
        }
    }
}
