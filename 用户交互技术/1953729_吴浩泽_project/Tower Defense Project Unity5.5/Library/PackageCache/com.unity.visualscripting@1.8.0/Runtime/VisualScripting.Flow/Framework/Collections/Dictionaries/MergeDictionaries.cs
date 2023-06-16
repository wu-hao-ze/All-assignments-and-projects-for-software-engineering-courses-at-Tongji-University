using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Merges two or more dictionaries together.
    /// </summary>
    /// <remarks>
    /// If the same key is found more than once, only the value
    /// of the first dictionary with this key will be used.
    /// </remarks>
    [UnitCategory("Collections/Dictionaries")]
    [UnitOrder(5)]
    public sealed class MergeDictionaries : MultiInputUnit<IDictionary>
    {
        /// <summary>
        /// The merged dictionary.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput dictionary { get; private set; }

        protected override void Definition()
        {
            dictionary = ValueOutput(nameof(dictionary), Merge);

            base.Definition();

            foreach (var input in multiInputs)
            {
                Requirement(input, dictionary);
            }
        }

        public IDictionary Merge(Flow flow)
        {
            var dictionary = new AotDictionary();

            for (var i = 0; i < inputCount; i++)
            {
                var inputDictionary = flow.GetValue<IDictionary>(multiInputs[i]);

                var enumerator = inputDictionary.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (!dictionary.Contains(enumerator.Key))
                    {
                        dictionary.Add(enumerator.Key, enumerator.Value);
                    }
                }
            }

            return dictionary;
        }
    }
}
