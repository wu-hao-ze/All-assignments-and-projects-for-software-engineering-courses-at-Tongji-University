using System;
using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Clears all items from a list.
    /// </summary>
    [UnitCategory("Collections/Lists")]
    [UnitSurtitle("List")]
    [UnitShortTitle("Clear")]
    [UnitOrder(6)]
    [TypeIcon(typeof(RemoveListItem))]
    public sealed class ClearList : Unit
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
        /// The cleared list.
        /// Note that the input list is modified directly and then returned,
        /// except if it is an array, in which case a new empty array
        /// is returned instead.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("List")]
        [PortLabelHidden]
        public ValueOutput listOutput { get; private set; }

        /// <summary>
        /// The action to execute once the list has been cleared.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Clear);
            listInput = ValueInput<IList>(nameof(listInput));
            listOutput = ValueOutput<IList>(nameof(listOutput));
            exit = ControlOutput(nameof(exit));

            Requirement(listInput, enter);
            Assignment(enter, listOutput);
            Succession(enter, exit);
        }

        public ControlOutput Clear(Flow flow)
        {
            var list = flow.GetValue<IList>(listInput);

            if (list is Array)
            {
                flow.SetValue(listOutput, Array.CreateInstance(list.GetType().GetElementType(), 0));
            }
            else
            {
                list.Clear();

                flow.SetValue(listOutput, list);
            }

            return exit;
        }
    }
}
