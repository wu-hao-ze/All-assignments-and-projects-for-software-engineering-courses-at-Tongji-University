using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IAotStubbable
    {
        IEnumerable<object> GetAotStubs(HashSet<object> visited);
    }
}
