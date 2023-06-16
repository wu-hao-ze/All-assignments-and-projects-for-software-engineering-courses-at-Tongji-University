using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public struct SkinnedColor
    {
        private static readonly bool isProSkin = EditorGUIUtility.isProSkin;

        public SkinnedColor(Color personalColor, Color proColor)
        {
            this.personalColor = personalColor;
            this.proColor = proColor;
        }

        public SkinnedColor(Color personalAndProColor)
        {
            personalColor = personalAndProColor;
            proColor = personalAndProColor;
        }

        private readonly Color personalColor;
        private readonly Color proColor;

        public Color color => isProSkin ? proColor : personalColor;

        public static implicit operator Color(SkinnedColor skinnedColor)
        {
            return skinnedColor.color;
        }

        public static explicit operator SkinnedColor(Color color)
        {
            return new SkinnedColor(color);
        }
    }
}
