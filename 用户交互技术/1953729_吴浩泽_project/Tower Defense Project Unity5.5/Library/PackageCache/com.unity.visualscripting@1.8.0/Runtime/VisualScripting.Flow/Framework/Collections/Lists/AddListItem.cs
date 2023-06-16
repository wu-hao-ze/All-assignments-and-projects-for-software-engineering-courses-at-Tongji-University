using System;
using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Adds an item to a list.
    /// </summary>
    [UnitCategory("Collections/Lists")]
    [UnitSurtitle("List")]
    [UnitShortTitle("Add Item")]
    [UnitOrder(2)]
    public sealed class AddListItem : Unit
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
        /// The list with the added element.
        /// Note that the input list is modified directly and then returned,
        /// except if it is an array, in which case a new array with
        /// the added element is returned instead.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("List")]
        [PortLabelHidden]
        public ValueOutput listOutput { get; private set; }

        /// <summary>
        /// The item to add.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput item { get; private set; }

        /// <summary>
        /// The action to execute once the item has been added.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Add);
            listInput = ValueInput<IList>(nameof(listInput));
            item = ValueInput<object>(nameof(item));
            listOutput = ValueOutput<IList>(nameof(listOutput));
            exit = ControlOutput(nameof(exit));

            Requirement(listInput, enter);
            Requirement(item, enter);
            Assignment(enter, listOutput);
            Succession(enter, exit);
        }

        public ControlOutput Add(Flow flow)
        {
            var list = flow.GetValue<IList>(listInput);
            var item = flow.GetValue<object>(this.item);

            if (list is Array)
            {
                var resizableList = new ArrayList(list);
                resizableList.Add(item);
                flow.SetValue(listOutput, resizableList.ToArray(list.GetType().GetElementType()));
            }
            else
            {
                list.Add(item);

                flow.SetValue(listOutput, list);
            }

            return exit;
        }
    }
}
