using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Unity.VisualScripting
{
    public sealed class Namespace
    {
        private Namespace(string fullName)
        {
            FullName = fullName;

            if (fullName != null)
            {
                var parts = fullName.Split('.');

                Name = parts[parts.Length - 1];

                if (parts.Length > 1)
                {
                    Root = parts[0];
                    Parent = fullName.Substring(0, fullName.LastIndexOf('.'));
                }
                else
                {
                    Root = this;
                    IsRoot = true;
                    Parent = Global;
                }
            }
            else
            {
                Root = this;
                IsRoot = true;
                IsGlobal = true;
            }
        }

        public Namespace Root { get; }
        public Namespace Parent { get; }
        public string FullName { get; }
        public string Name { get; }
        public bool IsRoot { get; }
        public bool IsGlobal { get; }

        public IEnumerable<Namespace> Ancestors
        {
            get
            {
                var ancestor = Parent;

                while (ancestor != null)
                {
                    yield return ancestor;
                    ancestor = ancestor.Parent;
                }
            }
        }

        public IEnumerable<Namespace> AndAncestors()
        {
            yield return this;

            foreach (var ancestor in Ancestors)
            {
                yield return ancestor;
            }
        }

        public override int GetHashCode()
        {
            if (FullName == null)
            {
                return 0;
            }

            return FullName.GetHashCode();
        }

        public override string ToString()
        {
            return FullName;
        }

        static Namespace()
        {
            collection = new Collection();
        }

        private static readonly Collection collection;

        public static Namespace Global { get; } = new Namespace(null);

        public static Namespace FromFullName(string fullName)
        {
            if (fullName == null)
            {
                return Global;
            }

            Namespace @namespace;

            if (!collection.TryGetValue(fullName, out @namespace))
            {
                @namespace = new Namespace(fullName);
                collection.Add(@namespace);
            }

            return @namespace;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Namespace;

            if (other == null)
            {
                return false;
            }

            return FullName == other.FullName;
        }

        public static implicit operator Namespace(string fullName)
        {
            return FromFullName(fullName);
        }

        public static implicit operator string(Namespace @namespace)
        {
            return @namespace.FullName;
        }

        public static bool operator ==(Namespace a, Namespace b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Namespace a, Namespace b)
        {
            return !(a == b);
        }

        private class Collection : KeyedCollection<string, Namespace>, IKeyedCollection<string, Namespace>
        {
            protected override string GetKeyForItem(Namespace item)
            {
                return item.FullName;
            }

            public new bool TryGetValue(string key, out Namespace value)
            {
                if (Dictionary == null)
                {
                    value = default(Namespace);
                    return false;
                }

                return Dictionary.TryGetValue(key, out value);
            }
        }
    }
}
