using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [PluginModule(required = true)]
    public class PluginResources : IPluginModule
    {
        protected PluginResources(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public virtual void Initialize()
        {
            //TODO: Move it to the lazy initialization

            if (plugin.id == BoltCore.ID)
            {
                _providers.Add(new EmbeddedResourceProvider());

                var pluginType = plugin.GetType();
                assembly = new AssemblyResourceProvider(pluginType.Assembly, pluginType.Namespace, assemblyRoot);

                _providers.Add(assembly);

                if (Directory.Exists(PluginPaths.resourcesFolder))
                {
                    editorAssets = new EditorAssetResourceProvider(PluginPaths.resourcesFolder);
                    _providers.Add(editorAssets);
                }

                if (File.Exists(PluginPaths.resourcesBundle))
                {
                    /*
                     * TODO: To be removed when the asset bundle team fix the issue JIRA: BOLT-1650
                     */
                    assetBundleResourceProvider = new AssetBundleResourceProvider();

                    _providers.Add(assetBundleResourceProvider);
                }

                if (_providers.Count == 0)
                {
                    Debug.LogWarning($"No plugin resources provider available for {plugin.id}.");
                }
                else
                {
                    defaultProvider = _providers[0];
                }
            }
        }

        internal void LoadMigrations()
        {
            migrations = InstantiateLinkedTypes<PluginMigration>().OrderBy(m => m).ToList().AsReadOnly();
        }

        public virtual void LateInitialize() { }

        public Plugin plugin { get; }

        #region Types

        public ReadOnlyCollection<PluginMigration> migrations { get; private set; }

        public IEnumerable<PluginMigration> pendingMigrations => migrations.Where(m => m.from >= plugin.manifest.savedVersion && m.to <= plugin.manifest.currentVersion);

        protected IEnumerable<Type> GetLinkedTypes<T>() where T : IPluginLinked
        {
            return PluginContainer.GetLinkedTypes(typeof(T), plugin.id);
        }

        protected T[] InstantiateLinkedTypes<T>() where T : IPluginLinked
        {
            return PluginContainer.InstantiateLinkedTypes(typeof(T), plugin).Cast<T>().ToArray();
        }

        #endregion


        #region Files

        public IResourceProvider defaultProvider { get; private set; }

        private static readonly List<IResourceProvider> _providers = new List<IResourceProvider>();

        public IEnumerable<IResourceProvider> providers => _providers;

        protected virtual string assemblyRoot => "Resources";

        public AssetBundleResourceProvider assetBundleResourceProvider { get; private set; }
        public AssemblyResourceProvider assembly { get; private set; }

        public EditorAssetResourceProvider editorAssets { get; private set; }

        public T LoadAsset<T>(string path, bool required) where T : UnityObject
        {
            foreach (var provider in providers)
            {
                var asset = provider.LoadAsset<T>(path);

                if (asset != null)
                {
                    return asset;
                }
            }

            if (required)
            {
                Debug.LogWarning($"Missing plugin asset: \n{path}.");
            }

            return null;
        }

        public EditorTexture LoadTexture(string path, CreateTextureOptions options, bool required = true)
        {
            return EditorTexture.Load(providers, path, options, required);
        }

        public EditorTexture LoadTexture(string path, TextureResolution[] resolutions, CreateTextureOptions options, bool required = true)
        {
            return EditorTexture.Load(providers, path, resolutions, options, required);
        }

        public EditorTexture LoadIcon(string path, bool required = true)
        {
            return EditorTexture.Load(providers, path, EditorTexture.StandardIconResolutions, CreateTextureOptions.PixelPerfect, required);
        }

        public static EditorTexture LoadSharedIcon(string path, bool required = true)
        {
            Ensure.That(nameof(path)).IsNotNull(path);

            if (PluginContainer.initialized)
            {
                foreach (var plugin in PluginContainer.plugins)
                {
                    var pluginIcon = plugin.resources.LoadIcon(path, false);

                    if (pluginIcon != null)
                    {
                        return pluginIcon;
                    }
                }
            }
            // Doing this only because we share an asset bundle of icons between all plugins.
            // Every plugin (including core) has an asset bundle resource provider that points to that pack, so
            // use this as a last ditch effort even if the whole plugin container isn't loaded.
            // Eventually, this should be refactored away (when we stop using a plugin container / individual plugins)
            else if (BoltCore.Resources != null)
            {
                var pluginIcon = BoltCore.Resources.LoadIcon(path, false);

                if (pluginIcon != null)
                {
                    return pluginIcon;
                }
            }

            if (required)
            {
                Debug.LogWarning($"Missing shared editor texture: \n{path}");
            }

            return null;
        }

        #endregion
    }
}
