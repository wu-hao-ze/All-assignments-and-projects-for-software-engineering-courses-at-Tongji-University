using System;
using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Removes the specified item from a list.
    /// </summary>
    [UnitCategory("Collections/Lists")]
    [UnitSurtitle("List")]
    [UnitShortTitle("Remove Item")]
    [UnitOrder(4)]
    public sealed class RemoveListItem : Unit
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
        [PortLabel("List")]
        [PortLabelHidden]
        public ValueInput listInput { get; private set; }

        /// <summary>
        /// The list without the removed item.
        /// Note that the input list is modified directly and then returned,
        /// except if it is an array, in which case a new array without the item
        /// is returned instead.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("List")]
        [PortLabelHidden]
        public ValueOutput listOutput { get; private set; }

        /// <summary>
        /// The item to remove.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput item { get; private set; }

        /// <summary>
        /// The action to execute once the item has been removed.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Remove);
            listInput = ValueInput<IList>(nameof(listInput));
            listOutput = ValueOutput<IList>(nameof(listOutput));
            item = ValueInput<object>(nameof(item));
            exit = ControlOutput(nameof(exit));

            Requirement(listInput, enter);
            Requirement(item, enter);
            Assignment(enter, listOutput);
            Succession(enter, exit);
        }

        public ControlOutput Remove(Flow flow)
        {
            var list = flow.GetValue<IList>(listInput);
            var item = flow.GetValue<object>(this.item);

            if (list is Array)
            {
                var resizableList = new ArrayList(list);
                resizableList.Remove(item);
                flow.SetValue(listOutput, resizableList.ToArray(list.GetType().GetElementType()));
            }
            else
            {
                list.Remove(item);
                flow.SetValue(listOutput, list);
            }

            return exit;
        }
    }
}
