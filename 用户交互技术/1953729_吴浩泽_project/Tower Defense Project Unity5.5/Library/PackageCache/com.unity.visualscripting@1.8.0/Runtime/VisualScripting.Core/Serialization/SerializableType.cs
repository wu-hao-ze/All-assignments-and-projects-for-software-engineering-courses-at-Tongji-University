using System;

namespace Unity.VisualScripting
{
    [Serializable]
    [SerializationVersion("A")]
    public struct SerializableType : IEquatable<SerializableType>, IComparable<SerializableType>
    {
        [Serialize]
        public string Identification;

        public SerializableType(string identification)
        {
            Identification = identification;
        }

        public bool Equals(SerializableType other)
        {
            return string.Equals(Identification, other.Identification);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SerializableType th && Equals(th);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Identification?.GetHashCode() ?? 0;
        }

        public static bool operator ==(SerializableType left, SerializableType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SerializableType left, SerializableType right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(SerializableType other)
        {
            return string.Compare(Identification, other.Identification, StringComparison.Ordinal);
        }
    }

    public class Unknown
    {
        public const string Identification = "__UNKNOWN";

        Unknown() { }
    }
}
