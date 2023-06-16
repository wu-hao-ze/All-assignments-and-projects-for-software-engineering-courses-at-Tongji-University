using System.Collections;
using System.Collections.Specialized;
using UnityEngine.Scripting;

namespace Unity.VisualScripting
{
    // Making this inherit OrderedDictionary for now because metadata
    // doesn't work well for unordered dictionaries. The ideal solution
    // would be to rework the MetadataDictionaryAdaptor to not require
    // the index at all, then make this inherit Dictionary<object, object>

    public sealed class AotDictionary : OrderedDictionary
    {
        public AotDictionary() : base() { }
        public AotDictionary(IEqualityComparer comparer) : base(comparer) { }
        public AotDictionary(int capacity) : base(capacity) { }
        public AotDictionary(int capacity, IEqualityComparer comparer) : base(capacity, comparer) { }

        [Preserve]
        public static void AotStubs()
        {
            var dictionary = new AotDictionary();

            dictionary.Add(default(object), default(object));
            dictionary.Remove(default(object));
            var item = dictionary[default(object)];
            dictionary[default(object)] = default(object);
            dictionary.Contains(default(object));
            dictionary.Clear();
            var count = dictionary.Count;
        }
    }
}
