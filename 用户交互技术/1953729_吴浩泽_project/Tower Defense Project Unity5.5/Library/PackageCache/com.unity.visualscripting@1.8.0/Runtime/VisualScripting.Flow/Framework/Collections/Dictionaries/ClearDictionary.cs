using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Clears all items from a dictionary.
    /// </summary>
    [UnitCategory("Collections/Dictionaries")]
    [UnitSurtitle("Dictionary")]
    [UnitShortTitle("Clear")]
    [UnitOrder(4)]
    [TypeIcon(typeof(RemoveDictionaryItem))]
    public sealed class ClearDictionary : Unit
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
        /// The cleared dictionary.
        /// Note that the input dictionary is modified directly and then returned.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Dictionary")]
        [PortLabelHidden]
        public ValueOutput dictionaryOutput { get; private set; }

        /// <summary>
        /// The action to execute once the dictionary has been cleared.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Clear);
            dictionaryInput = ValueInput<IDictionary>(nameof(dictionaryInput));
            dictionaryOutput = ValueOutput<IDictionary>(nameof(dictionaryOutput));
            exit = ControlOutput(nameof(exit));

            Requirement(dictionaryInput, enter);
            Assignment(enter, dictionaryOutput);
            Succession(enter, exit);
        }

        private ControlOutput Clear(Flow flow)
        {
            var dictionary = flow.GetValue<IDictionary>(dictionaryInput);

            flow.SetValue(dictionaryOutput, dictionary);

            dictionary.Clear();

            return exit;
        }
    }
}
