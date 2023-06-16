using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.VisualScripting
{
    [PluginModule(required = true)]
    public class PluginConfiguration : IPluginModule, IEnumerable<PluginConfigurationItemMetadata>
    {
        protected PluginConfiguration(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public virtual void Initialize()
        {
            Load();

#if VISUAL_SCRIPT_DEBUG_MIGRATION
            Debug.Log(
                $"Plugin Configuration for {this.plugin.id} init, load complete. Saved version is {savedVersion}");
#endif

            if (savedVersion != "0.0.0")
                return;

            // If our savedVersion is still 0.0.0, it means we didn't load an existing project savedVersion
            // Run any deprecatedSavedVersionLoaders we have to see if we can detect and load any older savedVersion formats in the project
            // Order by descending to find the latest possible savedVersion in the project first
            deprecatedSavedVersionLoaders = PluginContainer.InstantiateLinkedTypes(typeof(PluginDeprecatedSavedVersionLoader), plugin).
                Cast<PluginDeprecatedSavedVersionLoader>().ToArray().OrderByDescending(m => m.@from).ToList().AsReadOnly();

            foreach (var migration in deprecatedSavedVersionLoaders)
            {
                var success = migration.Run(out var loadedVersion);
                if (success)
                {
                    // Once we've found a valid savedVersion, we can break out and let the pluginMigrations run
                    savedVersion = loadedVersion;
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                    Debug.Log($"Found legacy version, loaded as {loadedVersion}");
#endif
                    return;
                }
            }

            // If we get here, it means we couldn't find any savedVersion in the project (legacy or not), it must be a fresh project
#if VISUAL_SCRIPT_DEBUG_MIGRATION
            Debug.Log($"Found no legacy versions for {this.plugin.id}, setting to {plugin.manifest.version}");
#endif
            projectSettingsAssetDirty = true;

            savedVersion = plugin.manifest.version;
        }

        public virtual void LateInitialize() { }

        public Plugin plugin { get; }

        public virtual string header => plugin.manifest.name;

        public ReadOnlyCollection<PluginDeprecatedSavedVersionLoader> deprecatedSavedVersionLoaders { get; private set; }

        #region Lifecycle

        private void Load()
        {
            LoadEditorPrefs();
            LoadProjectSettings();
        }

        public void Reset()
        {
            foreach (var item in allItems)
            {
                item.Reset();
            }
        }

        public void Save()
        {
            foreach (var item in allItems)
            {
                item.Save();
            }
        }

        #endregion


        #region All Items

        private IEnumerable<PluginConfigurationItemMetadata> allItems => LinqUtility.Concat<PluginConfigurationItemMetadata>(editorPrefs, projectSettings);

        public IEnumerator<PluginConfigurationItemMetadata> GetEnumerator()
        {
            return allItems.OrderBy(i => i.member.MetadataToken).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public PluginConfigurationItemMetadata GetMetadata(string memberName)
        {
            return allItems.First(metadata => metadata.member.Name == memberName);
        }

        #endregion


        #region Editor Prefs

        internal List<EditorPrefMetadata> editorPrefs;

        private void LoadEditorPrefs()
        {
            editorPrefs = new List<EditorPrefMetadata>();

            var metadata = Metadata.Root();

            foreach (var memberInfo in GetType().GetMembers().Where(f => f.HasAttribute<EditorPrefAttribute>()).OrderBy(m => m.MetadataToken))
            {
                editorPrefs.Add(metadata.EditorPref(this, memberInfo));
            }
        }

        #endregion


        #region Project Settings

        public List<ProjectSettingMetadata> projectSettings;

        private string projectSettingsStoragePath => PluginPaths.projectSettings;
        private bool projectSettingsAssetDirty = false;

        internal DictionaryAsset projectSettingsAsset { get; set; }

        internal void LoadProjectSettings()
        {
            LoadOrCreateProjectSettingsAsset();

            ResetProjectSettingsMetadata();
        }

        internal void ResetProjectSettingsMetadata()
        {
            projectSettings = new List<ProjectSettingMetadata>();

            var metadata = Metadata.Root();

            foreach (var memberInfo in GetType().GetMembers().Where(f => f.HasAttribute<ProjectSettingAttribute>()).OrderBy(m => m.MetadataToken))
            {
                projectSettings.Add(metadata.ProjectSetting(this, memberInfo));
            }
        }

        internal void LoadOrCreateProjectSettingsAsset()
        {
            if (File.Exists(projectSettingsStoragePath))
            {
                // Try loading the existing asset file.
                var objects = InternalEditorUtility.LoadSerializedFileAndForget(projectSettingsStoragePath);

                if (objects.Length <= 0 || objects[0] == null)
                {
                    // The file exists, but it isn't a valid asset.
                    // Warn and leave the asset as is to prevent losing its serialized contents
                    // because we might be able to salvage them by deserializing later on.
                    // Return a new empty instance in the mean time.
                    Debug.LogWarning($"Loading visual scripting project settings failed!");
                    projectSettingsAsset = ScriptableObject.CreateInstance<DictionaryAsset>();
                    return;
                }

                projectSettingsAsset = (DictionaryAsset)objects[0];
            }
            else
            {
                // The file doesn't exist, so create a new asset
                projectSettingsAsset = ScriptableObject.CreateInstance<DictionaryAsset>();
            }
        }

        public void SaveProjectSettingsAsset(bool immediately = false)
        {
            if (projectSettingsAssetDirty || immediately)
            {
                EditorApplication.delayCall += SerializeProjectSettingsAssetToDisk;
            }
        }

        private void SerializeProjectSettingsAssetToDisk()
        {
            if (VSUsageUtility.isVisualScriptingUsed)
            {
                // make sure the path exists or file write will fail
                PathUtility.CreateParentDirectoryIfNeeded(projectSettingsStoragePath);

                const bool saveAsText = true;
                InternalEditorUtility.SaveToSerializedFileAndForget(new UnityEngine.Object[] { projectSettingsAsset },
                    projectSettingsStoragePath, saveAsText);
            }
        }

        #endregion


        #region Items

        /// <summary>
        /// Whether the plugin was properly setup.
        /// </summary>
        [ProjectSetting(visibleCondition = nameof(developerMode), resettable = false)]
        public bool projectSetupCompleted { get; internal set; }

        /// <summary>
        /// Whether the plugin was properly setup.
        /// </summary>
        [EditorPref(visibleCondition = nameof(developerMode), resettable = false)]
        public bool editorSetupCompleted { get; internal set; }

        /// <summary>
        /// The last version to which the plugin successfully upgraded.
        /// </summary>
        [ProjectSetting(visibleCondition = nameof(developerMode), resettable = false)]
        public SemanticVersion savedVersion { get; internal set; }

        protected bool developerMode => BoltCore.Configuration.developerMode;

        #endregion


        #region Menu

#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Delete All Project Settings", priority = LudiqProduct.DeveloperToolsMenuPriority + 401)]
#endif
        public static void DeleteAllProjectSettings()
        {
            foreach (var plugin in PluginContainer.plugins)
            {
                AssetDatabase.DeleteAsset(PathUtility.FromProject(plugin.configuration.projectSettingsStoragePath));
            }
        }

#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Delete All Editor Prefs", priority = LudiqProduct.DeveloperToolsMenuPriority + 402)]
#endif
        public static void DeleteAllEditorPrefs()
        {
            foreach (var plugin in PluginContainer.plugins)
            {
                // Delete all current editor prefs for this plugin
                foreach (var editorPref in plugin.configuration.editorPrefs)
                {
                    EditorPrefs.DeleteKey(editorPref.namespacedKey);
                }

                // If our plugin was renamed, delete all editor pref keys for the plugin using its previous names
                IEnumerable<RenamedFromAttribute> renamedFromAttributes;
                var fieldInfo = plugin.GetType().GetField("ID", BindingFlags.Public | BindingFlags.Static);
                renamedFromAttributes = fieldInfo.GetCustomAttributes(typeof(RenamedFromAttribute), true).Cast<RenamedFromAttribute>();
                foreach (var renamed in renamedFromAttributes)
                {
                    foreach (var editorPref in plugin.configuration.editorPrefs)
                    {
                        EditorPrefs.DeleteKey(EditorPrefMetadata.GetNamespacedKey(renamed.previousName, editorPref.key));
                    }
                }
            }
        }

#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Delete All Player Prefs", priority = LudiqProduct.DeveloperToolsMenuPriority + 403)]
#endif
        public static void DeleteAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        #endregion
    }
}
