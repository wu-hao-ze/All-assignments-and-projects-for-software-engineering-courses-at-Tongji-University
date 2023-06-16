using System;
using System.Linq;

namespace Unity.VisualScripting
{
    public static class BoltFlowNameUtility
    {
        [Obsolete("This method is obsolete. Please use the new UnitTitle(unitType, short, includeStatus) instead.")]
        public static string UnitTitle(Type unitType, bool @short)
        {
            if (@short)
            {
                var shortTitle = unitType.GetAttribute<UnitShortTitleAttribute>()?.title;

                if (shortTitle != null)
                {
                    return shortTitle;
                }
            }

            var title = unitType.GetAttribute<UnitTitleAttribute>()?.title;

            if (title != null)
            {
                return title;
            }

            return unitType.HumanName();
        }

        public static string UnitTitle(Type unitType, bool @short, bool includeStatus)
        {
            var suffix = string.Empty;
            if (includeStatus && Attribute.IsDefined(unitType, typeof(ObsoleteAttribute)))
                suffix = " (Deprecated)";

            if (@short)
            {
                var shortTitle = unitType.GetAttribute<UnitShortTitleAttribute>()?.title;

                if (shortTitle != null)
                {
                    return $"{shortTitle} {suffix}";
                }
            }

            var title = unitType.GetAttribute<UnitTitleAttribute>()?.title;

            return title != null ? $"{title} {suffix}" : $"{unitType.HumanName()} {suffix}";
        }

        public static string UnitPreviousTitle(Type unitType)
        {
            var title = unitType.GetAttribute<RenamedFromAttribute>()?.previousName.Split('.').Last();
            return title ?? string.Empty;
        }
    }
}
