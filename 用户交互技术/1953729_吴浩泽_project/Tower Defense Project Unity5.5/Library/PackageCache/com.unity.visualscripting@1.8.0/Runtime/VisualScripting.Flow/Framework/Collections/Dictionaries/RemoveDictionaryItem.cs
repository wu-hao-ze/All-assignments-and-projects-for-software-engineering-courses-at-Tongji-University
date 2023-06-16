using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Removes a dictionary item with a specified key.
    /// </summary>
    [UnitCategory("Collections/Dictionaries")]
    [UnitSurtitle("Dictionary")]
    [UnitShortTitle("Remove Item")]
    [UnitOrder(3)]
    public sealed class RemoveDictionaryItem : Unit
    {
        /// <summary>
        /// The entry point for the node.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The dictionary.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Dictionary")]
        [PortLabelHidden]
        public ValueInput dictionaryInput { get; private set; }

        /// <summary>
        /// The dictionary without the removed item.
        /// Note that the input dictionary is modified directly and then returned.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Dictionary")]
        [PortLabelHidden]
        public ValueOutput dictionaryOutput { get; private set; }

        /// <summary>
        /// The key of the item to remove.
        /// </summary>
        [DoNotSerialize]
        public ValueInput key { get; private set; }

        /// <summary>
        /// The action to execute once the item has been removed.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Remove);
            dictionaryInput = ValueInput<IDictionary>(nameof(dictionaryInput));
            dictionaryOutput = ValueOutput<IDictionary>(nameof(dictionaryOutput));
            key = ValueInput<object>(nameof(key));
            exit = ControlOutput(nameof(exit));

            Requirement(dictionaryInput, enter);
            Requirement(key, enter);
            Assignment(enter, dictionaryOutput);
            Succession(enter, exit);
        }

        public ControlOutput Remove(Flow flow)
        {
            var dictionary = flow.GetValue<IDictionary>(dictionaryInput);
            var key = flow.GetValue<object>(this.key);

            flow.SetValue(dictionaryOutput, dictionary);

            dictionary.Remove(key);

            return exit;
        }
    }
}
