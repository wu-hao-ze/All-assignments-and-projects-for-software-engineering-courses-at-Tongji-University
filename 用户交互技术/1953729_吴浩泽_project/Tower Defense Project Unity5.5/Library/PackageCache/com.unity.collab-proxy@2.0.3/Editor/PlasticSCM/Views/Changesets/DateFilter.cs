using System;

namespace Unity.PlasticSCM.Editor.Views.Changesets
{
    internal class DateFilter
    {
        internal enum Type
        {
            LastWeek,
            Last15Days,
            LastMonth,
            Last3Months,
            LastYear,
            AllTime
        }

        internal Type FilterType;

        internal DateFilter(Type filterType)
        {
            FilterType = filterType;
        }

        internal DateTime GetFilterDate(DateTime referenceDate)
        {
            switch (FilterType)
            {
                case DateFilter.Type.LastWeek:
                    return referenceDate.AddDays(-7);
                case DateFilter.Type.Last15Days:
                    return referenceDate.AddDays(-15);
                case DateFilter.Type.LastMonth:
                    return referenceDate.AddMonths(-1);
                case DateFilter.Type.Last3Months:
                    return referenceDate.AddMonths(-3);
                case DateFilter.Type.LastYear:
                    return referenceDate.AddYears(-1);
            }

            return DateTime.MinValue;
        }
    }
}
