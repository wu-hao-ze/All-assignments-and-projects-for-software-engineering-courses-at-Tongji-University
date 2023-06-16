using System;
using System.Text.RegularExpressions;

namespace Unity.VisualScripting.Analytics
{
    internal static class AnalyticsUtilities
    {
        internal static string AnonymizeException(Exception e)
        {
            const string pathSectionOutsidePackage = "in (?<P>.*)Packages\\\\com.unity.visualscripting";
            // Placing a '^' character to distinguish these lines from file paths outside our package
            const string pathSectionOutsidePackageReplacement = "in ^Packages\\com.unity.visualscripting";

            var anonymizedString = Regex.Replace(e.ToString(), pathSectionOutsidePackage, pathSectionOutsidePackageReplacement);

            // Detecting any callstack line that doesn't match our previously anonymized package paths
            const string filePathsOutsidePackage = ". at.*in (?<P>[^^]*:[0-9]*)";
            const string filePathsOutsidePackageReplacement = "  at <method> in <file>";

            anonymizedString = Regex.Replace(anonymizedString, filePathsOutsidePackage,
                filePathsOutsidePackageReplacement);

            return anonymizedString;
        }
    }
}
