using System;
using System.Text.RegularExpressions;

namespace Unity.VisualScripting
{
    public partial class EnsureThat
    {
        public void IsNotNullOrWhiteSpace(string value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            IsNotNull(value);

            if (StringUtility.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(ExceptionMessages.Strings_IsNotNullOrWhiteSpace_Failed, paramName);
            }
        }

        public void IsNotNullOrEmpty(string value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            IsNotNull(value);

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(ExceptionMessages.Strings_IsNotNullOrEmpty_Failed, paramName);
            }
        }

        public void IsNotNull(string value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (value == null)
            {
                throw new ArgumentNullException(paramName, ExceptionMessages.Common_IsNotNull_Failed);
            }
        }

        public void IsNotEmpty(string value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (string.Empty.Equals(value))
            {
                throw new ArgumentException(ExceptionMessages.Strings_IsNotEmpty_Failed, paramName);
            }
        }

        public void HasLengthBetween(string value, int minLength, int maxLength)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            IsNotNull(value);

            var length = value.Length;

            if (length < minLength)
            {
                throw new ArgumentException(ExceptionMessages.Strings_HasLengthBetween_Failed_ToShort.Inject(minLength, maxLength, length), paramName);
            }

            if (length > maxLength)
            {
                throw new ArgumentException(ExceptionMessages.Strings_HasLengthBetween_Failed_ToLong.Inject(minLength, maxLength, length), paramName);
            }
        }

        public void Matches(string value, string match) => Matches(value, new Regex(match));

        public void Matches(string value, Regex match)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!match.IsMatch(value))
            {
                throw new ArgumentException(ExceptionMessages.Strings_Matches_Failed.Inject(value, match), paramName);
            }
        }

        public void SizeIs(string value, int expected)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            IsNotNull(value);

            if (value.Length != expected)
            {
                throw new ArgumentException(ExceptionMessages.Strings_SizeIs_Failed.Inject(expected, value.Length), paramName);
            }
        }

        public void IsEqualTo(string value, string expected)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!StringEquals(value, expected))
            {
                throw new ArgumentException(ExceptionMessages.Strings_IsEqualTo_Failed.Inject(value, expected), paramName);
            }
        }

        public void IsEqualTo(string value, string expected, StringComparison comparison)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!StringEquals(value, expected, comparison))
            {
                throw new ArgumentException(ExceptionMessages.Strings_IsEqualTo_Failed.Inject(value, expected), paramName);
            }
        }

        public void IsNotEqualTo(string value, string expected)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (StringEquals(value, expected))
            {
                throw new ArgumentException(ExceptionMessages.Strings_IsNotEqualTo_Failed.Inject(value, expected), paramName);
            }
        }

        public void IsNotEqualTo(string value, string expected, StringComparison comparison)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (StringEquals(value, expected, comparison))
            {
                throw new ArgumentException(ExceptionMessages.Strings_IsNotEqualTo_Failed.Inject(value, expected), paramName);
            }
        }

        public void IsGuid(string value)
        {
            if (!Ensure.IsActive)
            {
                return;
            }

            if (!StringUtility.IsGuid(value))
            {
                throw new ArgumentException(ExceptionMessages.Strings_IsGuid_Failed.Inject(value), paramName);
            }
        }

        private bool StringEquals(string x, string y, StringComparison? comparison = null)
        {
            return comparison.HasValue
                ? string.Equals(x, y, comparison.Value)
                : string.Equals(x, y);
        }
    }
}
