using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class ColorUtility
    {
        static ColorUtility()
        {
            pixels = new Dictionary<Color, Texture2D>();
        }

        private static readonly Dictionary<Color, Texture2D> pixels;

        public static Color Gray(float brightness)
        {
            return new Color(brightness, brightness, brightness);
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color WithAlphaMultiplied(this Color color, float alphaMultiplier)
        {
            color.a *= alphaMultiplier;
            return color;
        }

        public static Texture2D GetPixel(this Color color)
        {
            if (!pixels.ContainsKey(color))
            {
                string name = $"{EmbeddedResourceProvider.VISUAL_SCRIPTING_PACKAGE}.{color.ToHexString()}";

                Texture2D pixel = EmbeddedResourceProvider.CreatePixelTexture(name, color, 1, 1);

                pixels.Add(color, pixel);
            }

            return pixels[color];
        }

        public static Texture2D GetPixel(this SkinnedColor skinnedColor)
        {
            return skinnedColor.color.GetPixel();
        }

        public static Texture2D CreateBox(string name, Color fill, Color border)
        {
            Texture2D box = EmbeddedResourceProvider.LoadFromMemoryResources(name);

            if (box == null)
            {
                box = EmbeddedResourceProvider.CreatePixelTexture(name, border, 3, 3);

                box.SetPixel(1, 1, fill);
                box.Apply();
            }

            return box;
        }

        [Obsolete("Please use the new ColorUtility.CreateBox(name, fill, border) method instead.")]
        public static Texture2D CreateBox(Color fill, Color border)
        {
            var box = new Texture2D(3, 3, TextureFormat.ARGB32, false, LudiqGUIUtility.createLinearTextures);

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    box.SetPixel(i, j, border);
                }
            }

            box.SetPixel(1, 1, fill);
            box.hideFlags = HideFlags.HideAndDontSave;
            box.filterMode = FilterMode.Point;
            box.Apply();
            return box;
        }

        public static GUIStyle CreateBackground(this Color color)
        {
            var background = new GUIStyle();
            background.normal.background = color.GetPixel();
            return background;
        }

        public static GUIStyle CreateBackground(this SkinnedColor skinnedColor)
        {
            return skinnedColor.color.CreateBackground();
        }

        public static string ToHexString(this SkinnedColor skinnedColor)
        {
            return skinnedColor.color.ToHexString();
        }
    }
}
