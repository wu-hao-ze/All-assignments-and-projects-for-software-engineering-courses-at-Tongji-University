using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Adds an item to a dictionary.
    /// </summary>
    [UnitCategory("Collections/Dictionaries")]
    [UnitSurtitle("Dictionary")]
    [UnitShortTitle("Add Item")]
    [UnitOrder(2)]
    public sealed class AddDictionaryItem : Unit
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
        /// The dictionary with the added element.
        /// Note that the input dictionary is modified directly then returned.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Dictionary")]
        [PortLabelHidden]
        public ValueOutput dictionaryOutput { get; private set; }

        /// <summary>
        /// The key of the item to add.
        /// </summary>
        [DoNotSerialize]
        public ValueInput key { get; private set; }

        /// <summary>
        /// The value of the item to add.
        /// </summary>
        [DoNotSerialize]
        public ValueInput value { get; private set; }

        /// <summary>
        /// The action to execute once the item has been added.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Add);
            dictionaryInput = ValueInput<IDictionary>(nameof(dictionaryInput));
            key = ValueInput<object>(nameof(key));
            value = ValueInput<object>(nameof(value));
            dictionaryOutput = ValueOutput<IDictionary>(nameof(dictionaryOutput));
            exit = ControlOutput(nameof(exit));

            Requirement(dictionaryInput, enter);
            Requirement(key, enter);
            Requirement(value, enter);
            Assignment(enter, dictionaryOutput);
            Succession(enter, exit);
        }

        private ControlOutput Add(Flow flow)
        {
            var dictionary = flow.GetValue<IDictionary>(dictionaryInput);
            var key = flow.GetValue<object>(this.key);
            var value = flow.GetValue<object>(this.value);

            flow.SetValue(dictionaryOutput, dictionary);

            dictionary.Add(key, value);

            return exit;
        }
    }
}
