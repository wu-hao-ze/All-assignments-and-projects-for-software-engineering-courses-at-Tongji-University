using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Unity.VisualScripting
{
    public struct SemanticVersion : IComparable<SemanticVersion>
    {
        [Serialize]
        public readonly int major;

        [Serialize]
        public readonly int minor;

        [Serialize]
        public readonly int patch;

        [Serialize]
        public readonly string label;

        [Serialize]
        public readonly int increment;

        public SemanticVersion(int major, int minor, int patch, string label, int increment)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
            this.label = label;
            this.increment = increment;
        }

        public SemanticVersion(string semVerString)
        {
            this = Parse(semVerString);
        }

        public SemanticLabel semanticLabel
        {
            get
            {
                if (StringUtility.IsNullOrWhiteSpace(label))
                {
                    return SemanticLabel.Unspecified;
                }

                switch (label.Filter(whitespace: false, punctuation: false, symbols: false).ToLower())
                {
                    case "pre":
                        return SemanticLabel.Pre;

                    case "a":
                    case "alpha":
                        return SemanticLabel.Alpha;

                    case "b":
                    case "beta":
                        return SemanticLabel.Beta;

                    case "rc":
                    case "releasecandidate":
                        return SemanticLabel.ReleaseCandidate;

                    case "f":
                    case "final":
                    case "hotfix":
                    case "fix":
                        return SemanticLabel.Final;

                    default:
                        return SemanticLabel.Unspecified;
                }
            }
        }

        public override string ToString()
        {
            if (semanticLabel == SemanticLabel.Unspecified)
            {
                return $"{major}.{minor}.{patch}";
            }
            else
            {
                return $"{major}.{minor}.{patch}{label}{increment}";
            }
        }

        public static implicit operator SemanticVersion(string s)
        {
            return Parse(s);
        }

        public static SemanticVersion Parse(string s)
        {
            SemanticVersion result;

            if (!TryParse(s, out result))
            {
                throw new ArgumentException("s");
            }

            return result;
        }

        public static bool TryParse(string s, out SemanticVersion result)
        {
            result = default(SemanticVersion);

            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            var regex = new Regex(@"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(?:(?<label>[a-zA-Z\s\-_\.]+)(?<increment>\d+))?", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = regex.Match(s);

            if (!match.Success)
            {
                return false;
            }

            int major, minor, patch, increment = 0;
            string label = null;

            major = int.Parse(match.Groups["major"].Value);
            minor = int.Parse(match.Groups["minor"].Value);
            patch = int.Parse(match.Groups["patch"].Value);

            if (match.Groups["label"].Success)
            {
                label = match.Groups["label"].Value;
            }

            if (match.Groups["increment"].Success)
            {
                increment = int.Parse(match.Groups["increment"].Value);
            }

            result = new SemanticVersion(major, minor, patch, label, increment);

            return true;
        }

        // Final > _(Nothing)_ > Release Candidate > Beta > Alpha > Pre
        private static readonly Dictionary<SemanticLabel, int> LabelComparisonMap = new Dictionary<SemanticLabel, int>()
        {
            { SemanticLabel.Final, 6 },
            { SemanticLabel.Unspecified, 5 },
            { SemanticLabel.ReleaseCandidate, 4 },
            { SemanticLabel.Beta, 3 },
            { SemanticLabel.Alpha, 2 },
            { SemanticLabel.Pre, 1 },
        };

        public int CompareTo(SemanticVersion other)
        {
            var majorComparison = major.CompareTo(other.major);

            if (majorComparison != 0)
            {
                return majorComparison;
            }

            var minorComparison = minor.CompareTo(other.minor);

            if (minorComparison != 0)
            {
                return minorComparison;
            }

            var patchComparison = patch.CompareTo(other.patch);

            if (patchComparison != 0)
            {
                return patchComparison;
            }

            // Final > _(Nothing)_ > Release Candidate > Beta > Alpha > Pre
            var ours = LabelComparisonMap[this.semanticLabel];
            var others = LabelComparisonMap[other.semanticLabel];
            var labelComparison = ours.CompareTo(others);

            if (labelComparison != 0)
            {
                return labelComparison;
            }

            var incrementComparison = increment.CompareTo(other.increment);

            if (incrementComparison != 0)
            {
                return incrementComparison;
            }

            return 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SemanticVersion))
            {
                return false;
            }

            var other = (SemanticVersion)obj;

            return
                other.major == major &&
                other.minor == minor &&
                other.patch == patch &&
                other.semanticLabel == semanticLabel &&
                other.increment == increment;
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(major, minor, patch);
        }

        public static bool operator ==(SemanticVersion a, SemanticVersion b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SemanticVersion a, SemanticVersion b)
        {
            return !(a == b);
        }

        public static bool operator <(SemanticVersion a, SemanticVersion b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(SemanticVersion a, SemanticVersion b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(SemanticVersion a, SemanticVersion b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(SemanticVersion a, SemanticVersion b)
        {
            return a.CompareTo(b) >= 0;
        }

        public bool IsUnset()
        {
            return Equals(new SemanticVersion(0, 0, 0, null, 0));
        }
    }
}
