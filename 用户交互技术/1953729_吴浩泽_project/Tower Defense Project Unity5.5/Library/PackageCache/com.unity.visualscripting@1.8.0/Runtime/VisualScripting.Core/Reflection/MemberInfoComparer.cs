using System.Collections.Generic;
using System.Reflection;

namespace Unity.VisualScripting
{
    /// <summary>
    /// A member info comparer that will ignore the ReflectedType
    /// property by relying on the metadata token for comparison.
    /// </summary>
    public class MemberInfoComparer : EqualityComparer<MemberInfo>
    {
        public override bool Equals(MemberInfo x, MemberInfo y)
        {
            return x?.MetadataToken == y?.MetadataToken;
        }

        public override int GetHashCode(MemberInfo obj)
        {
            return obj.MetadataToken;
        }
    }
}
