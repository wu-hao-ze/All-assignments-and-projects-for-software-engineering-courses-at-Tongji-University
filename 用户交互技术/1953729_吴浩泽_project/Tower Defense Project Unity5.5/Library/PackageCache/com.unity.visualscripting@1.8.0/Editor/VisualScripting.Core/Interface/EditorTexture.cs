using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class EditorTexture
    {
        private EditorTexture()
        {
            personal = new Dictionary<int, Texture2D>();
            professional = new Dictionary<int, Texture2D>();
        }

        private EditorTexture(Texture2D texture) : this()
        {
            Ensure.That(nameof(texture)).IsNotNull(texture);

            personal.Add(texture.width, texture);
        }

        private EditorTexture(Texture texture) : this((Texture2D)texture) { }

        public static EditorTexture Single(Texture2D texture)
        {
            if (texture == null)
            {
                return null;
            }

            return new EditorTexture(texture);
        }

        public static EditorTexture Single(Texture texture)
        {
            if (texture == null)
            {
                return null;
            }

            return new EditorTexture(texture);
        }

        //TODO: remove once the asset bundle bug is fixed
        internal bool IsValid()
        {
            foreach (Texture2D texture2D in personal.Values)
            {
                if (texture2D != null)
                {
                    return true;
                }
            }

            foreach (Texture2D texture2D in professional.Values)
            {
                if (texture2D != null)
                {
                    return true;
                }
            }

            return false;
        }
        #region Fetching

        private string textureName;

        private readonly Dictionary<int, Texture2D> personal;

        private readonly Dictionary<int, Texture2D> professional;

        public Texture2D this[int resolution]
        {
            get
            {
                resolution = (int)(resolution * EditorGUIUtility.pixelsPerPoint);

                if (EditorGUIUtility.isProSkin)
                {
                    Texture2D proAtResolution;

                    if (!professional.TryGetValue(resolution, out proAtResolution))
                    {
                        if (professional.Count > 0)
                        {
                            proAtResolution = GetHighestResolution(professional);
                            professional.Add(resolution, proAtResolution);
                        }
                        else
                        {
                            Texture2D personalAtResolution;

                            if (!personal.TryGetValue(resolution, out personalAtResolution))
                            {
                                personalAtResolution = GetHighestResolution(personal);
                                personal.Add(resolution, personalAtResolution);
                            }

                            // if (personalAtResolution == null)
                            // {
                            //     Debug.Log($"{textureName} missing");
                            // }

                            return personalAtResolution;
                        }
                    }

                    // if (proAtResolution == null)
                    // {
                    //     Debug.Log($"{textureName} missing");
                    // }

                    return proAtResolution;
                }

                {
                    Texture2D personalAtResolution;

                    if (!personal.TryGetValue(resolution, out personalAtResolution))
                    {
                        personalAtResolution = GetHighestResolution(personal);
                        personal.Add(resolution, personalAtResolution);
                    }

                    // if (personalAtResolution == null)
                    // {
                    //     Debug.Log($"{textureName} missing");
                    // }

                    return personalAtResolution;
                }
            }
        }

        public Texture2D Single()
        {
            if (EditorGUIUtility.isProSkin)
            {
                if (professional.Count > 1)
                {
                    throw new InvalidOperationException();
                }
                else if (professional.Count == 1)
                {
                    return professional.Values.Single();
                }
            }

            if (personal.Count > 1)
            {
                throw new InvalidOperationException();
            }
            else if (personal.Count == 1)
            {
                return personal.Values.Single();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private Texture2D GetHighestResolution(Dictionary<int, Texture2D> dictionary)
        {
            return dictionary.OrderByDescending(kvp => kvp.Key).Select(kvp => kvp.Value).FirstOrDefault();
        }

        #endregion


        #region Loading

        public static readonly TextureResolution[] StandardIconResolutions = new TextureResolution[]
        {
            IconSize.Small,
            IconSize.Medium,
            IconSize.Large
        };

        public static EditorTexture Load(IEnumerable<IResourceProvider> resourceProviders, string path, CreateTextureOptions options, bool required)
        {
            foreach (var resources in resourceProviders)
            {
                var texture = Load(resources, path, options, false);

                if (texture != null)
                {
                    return texture;
                }
            }

            if (required)
            {
                var message = new StringBuilder();
                message.AppendLine("Missing editor texture: ");

                foreach (var resources in resourceProviders)
                {
                    message.AppendLine($"{resources.GetType().HumanName()}: {resources.DebugPath(path)}");
                }

                Debug.LogWarning(message.ToString());
            }

            return null;
        }

        public static EditorTexture Load(IEnumerable<IResourceProvider> resourceProviders, string path, TextureResolution[] resolutions, CreateTextureOptions options, bool required)
        {
            foreach (var resources in resourceProviders)
            {
                var texture = Load(resources, path, resolutions, options, false);

                if (texture != null)
                {
                    return texture;
                }
            }

            if (required)
            {
                var message = new StringBuilder();
                message.AppendLine("Missing editor texture: ");

                foreach (var resources in resourceProviders)
                {
                    message.AppendLine($"{resources.GetType().HumanName()}: {resources.DebugPath(path)}");
                }

                Debug.LogWarning(message.ToString());
            }

            return null;
        }

        public static EditorTexture Load(IResourceProvider resources, string path, CreateTextureOptions options, bool required)
        {
            using (ProfilingUtility.SampleBlock("Load Editor Texture"))
            {
                Ensure.That(nameof(resources)).IsNotNull(resources);
                Ensure.That(nameof(path)).IsNotNull(path);

                var set = new EditorTexture();
                var name = Path.GetFileNameWithoutExtension(path).PartBefore('@');
                var extension = Path.GetExtension(path);
                var directory = Path.GetDirectoryName(path);

                var personalPath = Path.Combine(directory, $"{name}{extension}");
                var professionalPath = Path.Combine(directory, $"{name}_Pro{extension}");

                var texture = resources.LoadTexture(personalPath, options);

                if (texture != null)
                {
                    set.personal.Add(texture.width, texture);
                }

                texture = resources.LoadTexture(professionalPath, options);

                if (texture != null)
                {
                    set.professional.Add(texture.width, texture);
                }

                if (set.personal.Count == 0)
                {
                    if (required)
                    {
                        Debug.LogWarning($"Missing editor texture: {name}\n{resources.DebugPath(path)}");
                    }

                    // Never return an empty set; the codebase assumes this guarantee

                    return null;
                }

                set.textureName = path;

                return set;
            }
        }

        public static EditorTexture Load(IResourceProvider resources, string path, TextureResolution[] resolutions, CreateTextureOptions options, bool required)
        {
            using (ProfilingUtility.SampleBlock("Load Editor Texture"))
            {
                Ensure.That(nameof(resources)).IsNotNull(resources);
                Ensure.That(nameof(path)).IsNotNull(path);
                Ensure.That(nameof(resolutions)).HasItems(resolutions);

                var set = new EditorTexture();

                // Try with explicit resolutions first
                foreach (var resolution in resolutions)
                {
                    var width = resolution.width;
                    // var height = resolution.height;

                    var personalPath = String.Empty;
                    var professionalPath = String.Empty;

                    personalPath = resources.GetPersonalPath(path, width);
                    professionalPath = resources.GetProfessionalPath(path, width);

                    if (resources.FileExists(personalPath))
                    {
                        var tex = resources.LoadTexture(personalPath, options);
                        set.personal.Add(width, tex);
                    }

                    if (resources.FileExists(professionalPath))
                    {
                        var tex = resources.LoadTexture(professionalPath, options);
                        set.professional.Add(width, tex);
                    }
                }

                if (set.personal.Count == 0)
                {
                    if (required)
                    {
                        Debug.LogWarning($"Missing editor texture: {path}\n{resources.DebugPath(path)}");
                    }

                    // Never return an empty set; the codebase assumes this guarantee

                    return null;
                }

                set.textureName = path;

                return set;
            }
        }

        #endregion
    }
}
