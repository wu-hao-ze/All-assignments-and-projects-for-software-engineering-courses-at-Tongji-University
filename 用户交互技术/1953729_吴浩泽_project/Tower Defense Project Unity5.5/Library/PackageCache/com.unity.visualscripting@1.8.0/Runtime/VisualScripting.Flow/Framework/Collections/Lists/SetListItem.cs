using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Sets the item at the specified index of a list.
    /// </summary>
    [UnitCategory("Collections/Lists")]
    [UnitSurtitle("List")]
    [UnitShortTitle("Set Item")]
    [UnitOrder(1)]
    [TypeIcon(typeof(IList))]
    public sealed class SetListItem : Unit
    {
        /// <summary>
        /// The entry point for the node.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

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
        public ValueInput item { get; private set; }

        /// <summary>
        /// The action to execute once the item has been assigned.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Set);
            list = ValueInput<IList>(nameof(list));
            index = ValueInput(nameof(index), 0);
            item = ValueInput<object>(nameof(item));
            exit = ControlOutput(nameof(exit));

            Requirement(list, enter);
            Requirement(index, enter);
            Requirement(item, enter);
            Succession(enter, exit);
        }

        public ControlOutput Set(Flow flow)
        {
            var list = flow.GetValue<IList>(this.list);
            var index = flow.GetValue<int>(this.index);
            var item = flow.GetValue<object>(this.item);

            list[index] = item;

            return exit;
        }
    }
}
