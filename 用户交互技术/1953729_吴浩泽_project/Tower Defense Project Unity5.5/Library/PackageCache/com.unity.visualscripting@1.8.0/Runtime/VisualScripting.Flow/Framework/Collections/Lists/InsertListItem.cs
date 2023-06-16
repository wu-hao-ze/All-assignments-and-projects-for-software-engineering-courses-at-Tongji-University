using System;
using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Inserts an item in a list at a specified index.
    /// </summary>
    [UnitCategory("Collections/Lists")]
    [UnitSurtitle("List")]
    [UnitShortTitle("Insert Item")]
    [UnitOrder(3)]
    [TypeIcon(typeof(AddListItem))]
    public sealed class InsertListItem : Unit
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
        /// The zero-based index at which to insert the item.
        /// </summary>
        [DoNotSerialize]
        public ValueInput index { get; private set; }

        /// <summary>
        /// The item to insert.
        /// </summary>
        [DoNotSerialize]
        public ValueInput item { get; private set; }

        /// <summary>
        /// The action to execute once the item has been inserted.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Insert);
            listInput = ValueInput<IList>(nameof(listInput));
            item = ValueInput<object>(nameof(item));
            index = ValueInput(nameof(index), 0);
            listOutput = ValueOutput<IList>(nameof(listOutput));
            exit = ControlOutput(nameof(exit));

            Requirement(listInput, enter);
            Requirement(item, enter);
            Requirement(index, enter);
            Assignment(enter, listOutput);
            Succession(enter, exit);
        }

        public ControlOutput Insert(Flow flow)
        {
            var list = flow.GetValue<IList>(listInput);
            var index = flow.GetValue<int>(this.index);
            var item = flow.GetValue<object>(this.item);

            if (list is Array)
            {
                var resizableList = new ArrayList(list);
                resizableList.Insert(index, item);
                flow.SetValue(listOutput, resizableList.ToArray(list.GetType().GetElementType()));
            }
            else
            {
                list.Insert(index, item);
                flow.SetValue(listOutput, list);
            }

            return exit;
        }
    }
}
