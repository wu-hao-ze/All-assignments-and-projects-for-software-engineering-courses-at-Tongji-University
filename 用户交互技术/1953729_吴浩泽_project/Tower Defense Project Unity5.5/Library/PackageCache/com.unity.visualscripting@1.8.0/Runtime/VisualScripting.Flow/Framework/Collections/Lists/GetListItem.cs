using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Gets the item at the specified index of a list.
    /// </summary>
    [UnitCategory("Collections/Lists")]
    [UnitSurtitle("List")]
    [UnitShortTitle("Get Item")]
    [UnitOrder(0)]
    [TypeIcon(typeof(IList))]
    public sealed class GetListItem : Unit
    {
        /// <summary>
        /// The list.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput list { get; private set; }

        /// <summary>
        /// The zero-based index.
        /// </summary>
        [DoNotSerialize]
        public ValueInput index { get; private set; }

        /// <summary>
        /// The item.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput item { get; private set; }

        protected override void Definition()
        {
            list = ValueInput<IList>(nameof(list));
            index = ValueInput(nameof(index), 0);
            item = ValueOutput(nameof(item), Get);

            Requirement(list, item);
            Requirement(index, item);
        }

        public object Get(Flow flow)
        {
            var list = flow.GetValue<IList>(this.list);
            var index = flow.GetValue<int>(this.index);

            return list[index];
        }
    }
}
