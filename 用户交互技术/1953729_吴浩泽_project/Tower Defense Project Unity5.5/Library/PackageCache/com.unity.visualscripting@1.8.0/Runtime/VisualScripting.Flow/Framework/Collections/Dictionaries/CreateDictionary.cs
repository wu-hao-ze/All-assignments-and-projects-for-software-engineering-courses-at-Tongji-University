using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Creates an empty dictionary.
    /// </summary>
    [UnitCategory("Collections/Dictionaries")]
    [UnitOrder(-1)]
    [TypeIcon(typeof(IDictionary))]
    [RenamedFrom("Bolt.CreateDitionary")]
    public sealed class CreateDictionary : Unit
    {
        /// <summary>
        /// The new empty dictionary.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput dictionary { get; private set; }

        protected override void Definition()
        {
            dictionary = ValueOutput(nameof(dictionary), Create);
        }

        public IDictionary Create(Flow flow)
        {
            return new AotDictionary();
        }
    }
}
