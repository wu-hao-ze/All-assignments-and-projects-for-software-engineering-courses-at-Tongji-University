using System;
using System.Collections;

namespace Unity.VisualScripting
{
    public sealed class DictionaryCloner : Cloner<IDictionary>
    {
        public override bool Handles(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        public override void FillClone(Type type, ref IDictionary clone, IDictionary original, CloningContext context)
        {
            // No support for instance preservation here, but none in FS either, so it shouldn't matter

            var originalEnumerator = original.GetEnumerator();

            while (originalEnumerator.MoveNext())
            {
                var originalKey = originalEnumerator.Key;
                var originalValue = originalEnumerator.Value;

                var cloneKey = Cloning.Clone(context, originalKey);
                var cloneValue = Cloning.Clone(context, originalValue);

                clone.Add(cloneKey, cloneValue);
            }
        }
    }
}
