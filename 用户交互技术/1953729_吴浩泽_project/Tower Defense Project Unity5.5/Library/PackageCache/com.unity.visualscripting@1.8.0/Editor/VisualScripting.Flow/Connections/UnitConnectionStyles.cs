using UnityEngine;

namespace Unity.VisualScripting
{
    public static class UnitConnectionStyles
    {
        public static readonly Color activeColor = new Color(0.37f, 0.66f, 0.95f);

        public static readonly Color highlightColor = new Color(1, 0.95f, 0f);

        public static readonly Color invalidColor = new Color(1, 0, 0);

        public static readonly Color disconnectColor = new Color(0.95f, 0.1f, 0.1f);

        public static readonly float minBend = 15;

        public static readonly float relativeBend = 1 / 4f;
    }
}
