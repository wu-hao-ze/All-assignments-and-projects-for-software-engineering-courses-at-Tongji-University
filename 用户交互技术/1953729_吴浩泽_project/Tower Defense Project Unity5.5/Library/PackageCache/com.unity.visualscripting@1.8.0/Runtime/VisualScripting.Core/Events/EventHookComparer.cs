using System.Collections.Generic;

namespace Unity.VisualScripting
{
    // Make sure the equality comparer doesn't use boxing
    public class EventHookComparer : IEqualityComparer<EventHook>
    {
        public bool Equals(EventHook x, EventHook y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(EventHook obj)
        {
            return obj.GetHashCode();
        }
    }
}
