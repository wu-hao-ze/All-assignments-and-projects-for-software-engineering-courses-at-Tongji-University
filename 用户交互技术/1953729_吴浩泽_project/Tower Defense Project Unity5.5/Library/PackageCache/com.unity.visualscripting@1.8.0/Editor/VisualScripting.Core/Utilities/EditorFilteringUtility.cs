namespace Unity.VisualScripting
{
    public static class EditorFilteringUtility
    {
        public static void Configure(this TypeFilter typeFilter, bool aotSafe = true)
        {
            if (aotSafe)
            {
                typeFilter.Abstract &= EditorPlatformUtility.allowJit;
                typeFilter.Interfaces &= EditorPlatformUtility.allowJit;
                typeFilter.Generic &= EditorPlatformUtility.allowJit;
                typeFilter.OpenConstructedGeneric &= EditorPlatformUtility.allowJit;
            }
        }

        public static void Configure(this MemberFilter memberFilter)
        {
            memberFilter.OpenConstructedGeneric &= EditorPlatformUtility.allowJit;
        }

        public static TypeFilter Configured(this TypeFilter typeFilter, bool aotSafe = true)
        {
            // We offer a bool to bypass AOT safety because of member *return type* filtering.
            // For instance, we want to allow methods that return an interface, even in AOT safe-mode,
            // because that method is guaranteed to be stubbable. However, we still want to restrict
            // methods *called* from an interface, because those can't be guaranteed stubbable.
            // https://support.ludiq.io/communities/5/topics/2458-aot-safe-mode-filters-return-types

            typeFilter = typeFilter.Clone();
            typeFilter.Configure(aotSafe);
            return typeFilter;
        }

        public static MemberFilter Configured(this MemberFilter memberFilter)
        {
            memberFilter = memberFilter.Clone();
            memberFilter.Configure();
            return memberFilter;
        }
    }
}
