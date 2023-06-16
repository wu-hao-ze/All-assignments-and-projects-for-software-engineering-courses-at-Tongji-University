using System;
using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Removes the item at the specified index of a list.
    /// </summary>
    [UnitCategory("Collections/Lists")]
    [UnitSurtitle("List")]
    [UnitShortTitle("Remove Item At Index")]
    [UnitOrder(5)]
    [TypeIcon(typeof(RemoveListItem))]
    public sealed class RemoveListItemAt : Unit
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
        public ValueInput listInput { get; private set; }

        /// <summary>
        /// The list without the removed item.
        /// Note that the input list is modified directly and then returned,
        /// except if it is an array, in which case a new array without the item
        /// is returned instead.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput listOutput { get; private set; }

        /// <summary>
        /// The zero-based index.
        /// </summary>
        [DoNotSerialize]
        public ValueInput index { get; private set; }

        /// <summary>
        /// The action to execute once the item has been removed.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), RemoveAt);
            listInput = ValueInput<IList>(nameof(listInput));
            listOutput = ValueOutput<IList>(nameof(listOutput));
            index = ValueInput(nameof(index), 0);
            exit = ControlOutput(nameof(exit));

            Requirement(listInput, enter);
            Requirement(index, enter);
            Assignment(enter, listOutput);
            Succession(enter, exit);
        }

        public ControlOutput RemoveAt(Flow flow)
        {
            var list = flow.GetValue<IList>(listInput);
            var index = flow.GetValue<int>(this.index);

            if (list is Array)
            {
                var resizableList = new ArrayList(list);
                resizableList.RemoveAt(index);
                flow.SetValue(listOutput, resizableList.ToArray(list.GetType().GetElementType()));
            }
            else
            {
                list.RemoveAt(index);
                flow.SetValue(listOutput, list);
            }

            return exit;
        }
    }
}
