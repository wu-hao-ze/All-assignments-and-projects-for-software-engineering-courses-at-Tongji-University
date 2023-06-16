using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI.Avatar
{
    internal static class ApplyCircleMask
    {
        internal static Texture2D For(Texture2D sourceImage)
        {
            int centerx = sourceImage.width / 2;
            int centery = sourceImage.height / 2;

            int radius = sourceImage.width / 2;

            Texture2D result = Images.GetNewTextureFromTexture(sourceImage);

            for (int i = (centerx - radius); i < centerx + radius; i++)
            {
                for (int j = (centery - radius); j < centery + radius; j++)
                {
                    float dx = i - centerx;
                    float dy = j - centery;

                    float d = Mathf.Sqrt(dx * dx + dy * dy);

                    float borderSize = 1f;

                    if (d <= (radius - borderSize))
                    {
                        result.SetPixel(
                            i - (centerx - radius),
                            j - (centery - radius),
                            sourceImage.GetPixel(i, j));
                        continue;
                    }

                    Color color = sourceImage.GetPixel(i, j);

                    result.SetPixel(
                        i - (centerx - radius),
                        j - (centery - radius),
                        Color.Lerp(Color.clear, color,
                            GetAntialiasAlpha(radius, d, borderSize)));
                }
            }

            result.Apply();

            return result;
        }

        static float GetAntialiasAlpha(float radius, float d, float borderSize)
        {
            if (d >= (radius + borderSize))
                return 0f;

            if (d - radius - borderSize == 0)
                return 0;

            float proportion =
                Mathf.Abs(d - radius - borderSize) /
                (radius + borderSize) - (radius - borderSize);

            return Mathf.Max(0, 1.0f - proportion);
        }
    }
}