using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.TextureAssets;
using UnityEngine;

namespace Unity.VisualScripting
{
    internal sealed class EmbeddedResourceProvider : IResourceProvider
    {
        internal const string VISUAL_SCRIPTING_PACKAGE = "com.unity.visualscripting";
        private static readonly Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();
        private readonly HashSet<string> cachedResources = new HashSet<string>();

        public EmbeddedResourceProvider()
        {
            cache.Clear();
        }

        internal static Texture2D LoadFromMemoryResources(string name)
        {
            if (!name.StartsWith(VISUAL_SCRIPTING_PACKAGE))
            {
                name = $"{VISUAL_SCRIPTING_PACKAGE}.{name}";
            }

            if (cache.Count == 0)
            {
                Texture2D[] arrayOfTexture2D = Resources.FindObjectsOfTypeAll<Texture2D>();

                foreach (Texture2D asset in arrayOfTexture2D)
                {
                    if (asset.name.StartsWith(VISUAL_SCRIPTING_PACKAGE))
                    {
                        cache[asset.name] = asset;
                    }
                }
            }

            Texture2D texture2D;

            cache.TryGetValue(name, out texture2D);

            return texture2D;
        }

        public IEnumerable<string> GetAllFiles()
        {
            return ResourceLoader.ListAllResources();
        }

        public IEnumerable<string> GetFiles(string path)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            throw new System.NotImplementedException();
        }

        string IResourceProvider.GetPersonalPath(string path, float width)
        {
            var name = Path.GetFileNameWithoutExtension(path).PartBefore('@');
            var extension = Path.GetExtension(path);
            var directory = Path.GetDirectoryName(path);

            return $"{name}@{width}x{extension}";
        }

        public string GetProfessionalPath(string path, float width)
        {
            var name = Path.GetFileNameWithoutExtension(path).PartBefore('@');
            var extension = Path.GetExtension(path);

            return $"{name}_Pro@{width}x{extension}";
        }

        public bool FileExists(string path)
        {
            if (cachedResources.Count == 0)
                foreach (string resource in ResourceLoader.ListAllResources())
                    cachedResources.Add(resource.Trim());

            return cachedResources.Contains(ResourceLoader.NormalizerPath(path).Trim());
        }

        public bool DirectoryExists(string path)
        {
            throw new System.NotImplementedException();
        }

        public string DebugPath(string path)
        {
            return path;
        }

        /// <summary>
        /// Create 1x1 pixel texture of specified color.
        /// </summary>
        /// <param name="name">Name for texture object.</param>
        /// <param name="color">Pixel color.</param>
        /// <returns>
        /// The new <c>Texture2D</c> instance.
        /// </returns>
        internal static Texture2D CreatePixelTexture(string name, Color color, int width, int height)
        {
            if (!name.StartsWith(VISUAL_SCRIPTING_PACKAGE))
            {
                name = $"{VISUAL_SCRIPTING_PACKAGE}.{name}";
            }

            Texture2D texture2D = LoadFromMemoryResources(name);

            if (texture2D == null)
            {
                texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false, LudiqGUIUtility.createLinearTextures);
                texture2D.name = name;
                texture2D.hideFlags = HideFlags.HideAndDontSave;
                texture2D.filterMode = FilterMode.Point;
                texture2D.SetPixel(0, 0, color);
                texture2D.Apply();

                cache[name] = texture2D;
            }

            return texture2D;
        }

        public T LoadAsset<T>(string path) where T : Object
        {
            throw new System.NotImplementedException();
        }

        public Texture2D LoadTexture(string path, CreateTextureOptions options)
        {
            path = ResourceLoader.NormalizerPath(path);

            string name = $"{VISUAL_SCRIPTING_PACKAGE}.{path}";

            Texture2D texture = LoadFromMemoryResources(name);

            if (texture == null)
            {
                byte[] textureData = ResourceLoader.LoadTexture(path);

                texture = new Texture2D(0, 0);
                texture.name = name;
                texture.alphaIsTransparency = options.alphaIsTransparency;
                texture.filterMode = options.filterMode;
                texture.hideFlags = options.hideFlags;
                texture.LoadImage(textureData);

                cache[name] = texture;
            }

            return texture;
        }
    }
}
