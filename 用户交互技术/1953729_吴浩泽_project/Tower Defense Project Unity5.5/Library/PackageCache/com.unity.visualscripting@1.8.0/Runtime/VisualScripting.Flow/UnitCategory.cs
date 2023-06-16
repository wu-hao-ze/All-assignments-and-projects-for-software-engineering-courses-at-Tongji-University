using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class)]
    [fsObject(Converter = typeof(UnitCategoryConverter))]
    public class UnitCategory : Attribute
    {
        public UnitCategory(string fullName)
        {
            Ensure.That(nameof(fullName)).IsNotNull(fullName);

            fullName = fullName.Replace('\\', '/');

            this.fullName = fullName;

            var parts = fullName.Split('/');

            name = parts[parts.Length - 1];

            if (parts.Length > 1)
            {
                root = new UnitCategory(parts[0]);
                parent = new UnitCategory(fullName.Substring(0, fullName.LastIndexOf('/')));
            }
            else
            {
                root = this;
                isRoot = true;
            }
        }

        public UnitCategory root { get; }
        public UnitCategory parent { get; }
        public string fullName { get; }
        public string name { get; }
        public bool isRoot { get; }

        public IEnumerable<UnitCategory> ancestors
        {
            get
            {
                var ancestor = parent;

                while (ancestor != null)
                {
                    yield return ancestor;
                    ancestor = ancestor.parent;
                }
            }
        }

        public IEnumerable<UnitCategory> AndAncestors()
        {
            yield return this;

            foreach (var ancestor in ancestors)
            {
                yield return ancestor;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is UnitCategory && ((UnitCategory)obj).fullName == fullName;
        }

        public override int GetHashCode()
        {
            return fullName.GetHashCode();
        }

        public override string ToString()
        {
            return fullName;
        }

        public static bool operator ==(UnitCategory a, UnitCategory b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(UnitCategory a, UnitCategory b)
        {
            return !(a == b);
        }
    }
}
