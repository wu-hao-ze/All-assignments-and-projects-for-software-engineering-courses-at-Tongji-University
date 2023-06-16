namespace Unity.VisualScripting
{
    public static class HashUtility
    {
        public static int GetHashCode<T>(T a)
        {
            return a?.GetHashCode() ?? 0;
        }

        public static int GetHashCode<T1, T2>(T1 a, T2 b)
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + (a?.GetHashCode() ?? 0);
                hash = hash * 23 + (b?.GetHashCode() ?? 0);

                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3>(T1 a, T2 b, T3 c)
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + (a?.GetHashCode() ?? 0);
                hash = hash * 23 + (b?.GetHashCode() ?? 0);
                hash = hash * 23 + (c?.GetHashCode() ?? 0);

                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3, T4>(T1 a, T2 b, T3 c, T4 d)
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + (a?.GetHashCode() ?? 0);
                hash = hash * 23 + (b?.GetHashCode() ?? 0);
                hash = hash * 23 + (c?.GetHashCode() ?? 0);
                hash = hash * 23 + (d?.GetHashCode() ?? 0);

                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3, T4, T5>(T1 a, T2 b, T3 c, T4 d, T5 e)
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + (a?.GetHashCode() ?? 0);
                hash = hash * 23 + (b?.GetHashCode() ?? 0);
                hash = hash * 23 + (c?.GetHashCode() ?? 0);
                hash = hash * 23 + (d?.GetHashCode() ?? 0);
                hash = hash * 23 + (e?.GetHashCode() ?? 0);

                return hash;
            }
        }

        public static int GetHashCodeAlloc(params object[] values)
        {
            unchecked
            {
                var hash = 17;

                foreach (var value in values)
                {
                    hash = hash * 23 + (value?.GetHashCode() ?? 0);
                }

                return hash;
            }
        }
    }
}
