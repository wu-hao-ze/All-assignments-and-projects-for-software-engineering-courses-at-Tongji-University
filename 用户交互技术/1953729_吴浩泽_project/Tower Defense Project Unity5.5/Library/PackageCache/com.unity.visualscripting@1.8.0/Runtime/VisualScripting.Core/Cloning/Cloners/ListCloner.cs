using System;
using System.Collections;

namespace Unity.VisualScripting
{
    public sealed class ListCloner : Cloner<IList>
    {
        public override bool Handles(Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }

        public override void FillClone(Type type, ref IList clone, IList original, CloningContext context)
        {
            if (context.tryPreserveInstances)
            {
                for (int i = 0; i < original.Count; i++)
                {
                    var originalItem = original[i];

                    if (i < clone.Count)
                    {
                        // The clone has at least as many items, we can try to preserve its instances.
                        var cloneItem = clone[i];
                        Cloning.CloneInto(context, ref cloneItem, originalItem);
                        clone[i] = cloneItem;
                    }
                    else
                    {
                        // The clone has less items than the original, we have to add a new item.
                        clone.Add(Cloning.Clone(context, originalItem));
                    }
                }
            }
            else
            {
                // Avoiding foreach to avoid enumerator allocation

                for (int i = 0; i < original.Count; i++)
                {
                    var originalItem = original[i];

                    clone.Add(Cloning.Clone(context, originalItem));
                }
            }
        }
    }
}
