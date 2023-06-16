using System;
using System.Reflection;

namespace Unity.VisualScripting
{
    public struct LooseAssemblyName
    {
        public readonly string name;

        public LooseAssemblyName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.name = name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is LooseAssemblyName))
            {
                return false;
            }

            return ((LooseAssemblyName)obj).name == name;
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(name);
        }

        public static bool operator ==(LooseAssemblyName a, LooseAssemblyName b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(LooseAssemblyName a, LooseAssemblyName b)
        {
            return !(a == b);
        }

        public static implicit operator LooseAssemblyName(string name)
        {
            return new LooseAssemblyName(name);
        }

        public static implicit operator string(LooseAssemblyName name)
        {
            return name.name;
        }

        public static explicit operator LooseAssemblyName(AssemblyName strongAssemblyName)
        {
            return new LooseAssemblyName(strongAssemblyName.Name);
        }

        public override string ToString()
        {
            return name;
        }
    }
}
