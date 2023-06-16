namespace Unity.VisualScripting
{
    public static class ExceptionMessages
    {
        public static string Common_IsNull_Failed { get; } = "Value must be null.";
        public static string Common_IsNotNull_Failed { get; } = "Value cannot be null.";

        public static string Booleans_IsTrueFailed { get; } = "Expected an expression that evaluates to true.";
        public static string Booleans_IsFalseFailed { get; } = "Expected an expression that evaluates to false.";

        public static string Collections_Any_Failed { get; } = "The predicate did not match any elements.";
        public static string Collections_ContainsKey_Failed { get; } = "{1} '{0}' was not found.";
        public static string Collections_HasItemsFailed { get; } = "Empty collection is not allowed.";
        public static string Collections_HasNoNullItemFailed { get; } = "Collection with null items is not allowed.";
        public static string Collections_SizeIs_Failed { get; } = "Expected size '{0}' but found '{1}'.";

        public static string Comp_Is_Failed { get; } = "Value '{0}' is not '{1}'.";
        public static string Comp_IsNot_Failed { get; } = "Value '{0}' is '{1}', which was not expected.";
        public static string Comp_IsNotLt { get; } = "Value '{0}' is not lower than limit '{1}'.";
        public static string Comp_IsNotLte { get; } = "Value '{0}' is not lower than or equal to limit '{1}'.";
        public static string Comp_IsNotGt { get; } = "Value '{0}' is not greater than limit '{1}'.";
        public static string Comp_IsNotGte { get; } = "Value '{0}' is not greater than or equal to limit '{1}'.";
        public static string Comp_IsNotInRange_ToLow { get; } = "Value '{0}' is < min '{1}'.";
        public static string Comp_IsNotInRange_ToHigh { get; } = "Value '{0}' is > max '{1}'.";

        public static string Guids_IsNotEmpty_Failed { get; } = "An empty GUID is not allowed.";

        public static string Strings_IsEqualTo_Failed { get; } = "Value '{0}' is not '{1}'.";
        public static string Strings_IsNotEqualTo_Failed { get; } = "Value '{0}' is '{1}', which was not expected.";
        public static string Strings_SizeIs_Failed { get; } = "Expected length '{0}' but got '{1}'.";
        public static string Strings_IsNotNullOrWhiteSpace_Failed { get; } = "The string can't be left empty, null or consist of only whitespaces.";
        public static string Strings_IsNotNullOrEmpty_Failed { get; } = "The string can't be null or empty.";
        public static string Strings_HasLengthBetween_Failed_ToShort { get; } = "The string is not long enough. Must be between '{0}' and '{1}' but was '{2}' characters long.";
        public static string Strings_HasLengthBetween_Failed_ToLong { get; } = "The string is too long. Must be between '{0}' and  '{1}'. Must be between '{0}' and '{1}' but was '{2}' characters long.";
        public static string Strings_Matches_Failed { get; } = "Value '{0}' does not match '{1}'";
        public static string Strings_IsNotEmpty_Failed { get; } = "Empty String is not allowed.";
        public static string Strings_IsGuid_Failed { get; } = "Value '{0}' is not a valid GUID.";

        public static string Types_IsOfType_Failed { get; } = "Expected a '{0}' but got '{1}'.";

        public static string Reflection_HasAttribute_Failed { get; } = "Type '{0}' does not define the [{1}] attribute.";
        public static string Reflection_HasConstructor_Failed { get; } = "Type '{0}' does not provide a constructor accepting ({1}).";
        public static string Reflection_HasPublicConstructor_Failed { get; } = "Type '{0}' does not provide a public constructor accepting ({1}).";

        public static string ValueTypes_IsNotDefault_Failed { get; } = "The param was expected to not be of default value.";
    }
}
