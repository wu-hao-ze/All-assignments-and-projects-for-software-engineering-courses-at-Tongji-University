namespace Unity.VisualScripting
{
    public struct EventHook
    {
        public readonly string name;

        public readonly object target;

        public readonly object tag;

        public EventHook(string name, object target = null, object tag = null)
        {
            Ensure.That(nameof(name)).IsNotNull(name);

            this.name = name;
            this.target = target;
            this.tag = tag;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EventHook other))
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(EventHook other)
        {
            return name == other.name && Equals(target, other.target) && Equals(tag, other.tag);
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(name, target, tag);
        }

        public static bool operator ==(EventHook a, EventHook b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(EventHook a, EventHook b)
        {
            return !(a == b);
        }

        public static implicit operator EventHook(string name)
        {
            return new EventHook(name);
        }
    }
}
