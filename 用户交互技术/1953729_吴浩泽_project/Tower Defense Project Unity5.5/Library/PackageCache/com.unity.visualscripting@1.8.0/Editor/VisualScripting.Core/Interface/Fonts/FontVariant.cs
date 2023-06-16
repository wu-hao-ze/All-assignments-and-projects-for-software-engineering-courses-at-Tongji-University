using UnityEngine;

namespace Unity.VisualScripting
{
    public struct FontVariant
    {
        public readonly FontWeight weight;
        public readonly FontStyle style;

        public FontVariant(FontWeight weight, FontStyle style)
        {
            this.weight = weight;
            this.style = style;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FontVariant))
            {
                return false;
            }

            var other = (FontVariant)obj;

            return weight == other.weight && style == other.style;
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(weight, style);
        }
    }
}
