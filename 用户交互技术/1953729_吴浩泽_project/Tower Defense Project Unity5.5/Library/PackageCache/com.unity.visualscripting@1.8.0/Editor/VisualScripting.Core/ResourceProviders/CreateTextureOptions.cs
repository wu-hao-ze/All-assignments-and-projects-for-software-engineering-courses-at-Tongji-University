using UnityEngine;

namespace Unity.VisualScripting
{
    public struct CreateTextureOptions
    {
        public bool alphaIsTransparency { get; set; }

        public bool mipmaps { get; set; }

        public TextureFormat textureFormat { get; set; }

        public FilterMode filterMode { get; set; }

        public HideFlags hideFlags { get; set; }

        public bool? linear { get; set; }

        public static readonly CreateTextureOptions PixelPerfect = new CreateTextureOptions()
        {
            alphaIsTransparency = true,
            mipmaps = false,
            textureFormat = TextureFormat.ARGB32,
            filterMode = FilterMode.Bilinear,
            hideFlags = HideFlags.HideAndDontSave
        };

        public static readonly CreateTextureOptions Scalable = new CreateTextureOptions()
        {
            alphaIsTransparency = true,
            mipmaps = true,
            textureFormat = TextureFormat.ARGB32,
            filterMode = FilterMode.Bilinear,
            hideFlags = HideFlags.HideAndDontSave
        };
    }
}
