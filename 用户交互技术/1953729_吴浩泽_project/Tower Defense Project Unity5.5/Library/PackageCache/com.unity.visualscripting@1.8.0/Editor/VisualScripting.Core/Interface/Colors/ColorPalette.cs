using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class ColorPalette
    {
        // Unity

        public static SkinnedColor unityBackgroundVeryDark = new SkinnedColor(ColorUtility.Gray(0.33f), ColorUtility.Gray(0.10f));
        public static SkinnedColor unityBackgroundDark = new SkinnedColor(ColorUtility.Gray(0.64f), ColorUtility.Gray(0.16f));
        public static SkinnedColor unityBackgroundMid = new SkinnedColor(ColorUtility.Gray(0.76f), ColorUtility.Gray(0.22f));
        public static SkinnedColor unityBackgroundLight = new SkinnedColor(ColorUtility.Gray(0.87f), ColorUtility.Gray(0.24f));
        public static SkinnedColor unityBackgroundLighter = new SkinnedColor(ColorUtility.Gray(0.87f * 1.1f), ColorUtility.Gray(0.24f * 1.1f));
        public static SkinnedColor unityBackgroundPure = new SkinnedColor(Color.white, Color.black);
        public static SkinnedColor unityForeground = new SkinnedColor(ColorUtility.Gray(0.00f), ColorUtility.Gray(0.81f));
        public static SkinnedColor unityForegroundDim = new SkinnedColor(ColorUtility.Gray(0.38f), ColorUtility.Gray(0.50f));
        public static SkinnedColor unityForegroundSelected = new SkinnedColor(ColorUtility.Gray(1.00f), ColorUtility.Gray(1.00f));
        public static SkinnedColor unitySelectionHighlight = new SkinnedColor(new Color(0.24f, 0.49f, 0.91f), new Color(0.20f, 0.38f, 0.57f));

        // Rotorz

        public static SkinnedColor reorderableListBackground = new SkinnedColor(ColorUtility.Gray(0.83f), ColorUtility.Gray(0.22f));

        // Utility

        public static SkinnedColor transparent = new SkinnedColor(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0));
        public static SkinnedColor hyperlink = new SkinnedColor(Color.blue, new Color(0.34f, 0.61f, 0.84f));
        public static SkinnedColor hyperlinkActive = new SkinnedColor(Color.red, Color.white);
    }
}
