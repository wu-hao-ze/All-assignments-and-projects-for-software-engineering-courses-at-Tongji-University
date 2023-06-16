using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class MigrationUtility_Asset_to_Package
    {
        public static DictionaryAsset GetLegacyProjectSettingsAsset(string pluginId)
        {
            try
            {
                var rootPath = GetLegacyRootPath(pluginId);
                var settingsFullPath = Path.Combine(rootPath, "Generated", "ProjectSettings.asset");
                var settingsAssetPath = Path.Combine("Assets", PathUtility.FromAssets(settingsFullPath));
                var asset = AssetDatabase.LoadAssetAtPath<DictionaryAsset>(settingsAssetPath);
                return asset;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetLegacyIconMapAssetPath(string pluginId)
        {
            try
            {
                var rootPath = GetLegacyRootPath(pluginId);
                var iconMapFullPath = Path.Combine(rootPath, "IconMap");
                var iconMapAssetPath = Path.Combine("Assets", PathUtility.FromAssets(iconMapFullPath));
                return iconMapAssetPath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetLegacyRootPath(string pluginId)
        {
            var rootFileName = $"{pluginId}.root";
            var defaultRootFolderPath = Path.Combine(Paths.assets, "Ludiq", pluginId);
            // Quick & dirty optimization: looking in all directories is expensive,
            // so if the user left the plugin in the default directory that we ship
            // (directly under Plugins), we'll use this path directly.

            string rootFilePath;

            var defaultRootFilePath = Path.Combine(defaultRootFolderPath, rootFileName);

            if (File.Exists(defaultRootFilePath))
            {
                rootFilePath = defaultRootFilePath;
            }
            else
            {
                var rootFiles = Directory.GetFiles(Paths.assets, rootFileName, SearchOption.AllDirectories);

                if (rootFiles.Length > 1)
                {
                    throw new IOException($"More than one root files found ('{rootFileName}'). Cannot determine root path.");
                }
                else if (rootFiles.Length <= 0)
                {
                    throw new FileNotFoundException($"No root file found ('{rootFileName}'). Cannot determine root path.");
                }
                else // if (rootFiles.Length == 1)
                {
                    rootFilePath = rootFiles[0];
                }
            }

            return Directory.GetParent(rootFilePath).FullName;
        }

        public static SemanticVersion TryManualParseSavedVersion(string pluginId)
        {
            try
            {
                var oldProjectRootPath = MigrationUtility_Asset_to_Package.GetLegacyRootPath(pluginId);
                var oldProjectSettingsPath = Path.Combine(oldProjectRootPath, "Generated", "ProjectSettings.asset");

                if (!File.Exists(oldProjectSettingsPath))
                {
                    return new SemanticVersion();
                }

                string projectSettingsText = System.IO.File.ReadAllText(oldProjectSettingsPath);
                int savedVersionIndex = projectSettingsText.IndexOf("savedVersion", StringComparison.Ordinal);
                if (savedVersionIndex == -1)
                {
                    return new SemanticVersion();
                }

                Match majorVersionMatch = new Regex(@"""major"":([0-9]*),").Match(projectSettingsText, savedVersionIndex);
                Match minorVersionMatch = new Regex(@"""minor"":([0-9]*),").Match(projectSettingsText, savedVersionIndex);
                Match patchVersionMatch = new Regex(@"""patch"":([0-9]*),").Match(projectSettingsText, savedVersionIndex);

                int majorVersion = int.Parse(majorVersionMatch.Groups[1].Value);
                int minorVersion = int.Parse(minorVersionMatch.Groups[1].Value);
                int patchVersion = int.Parse(patchVersionMatch.Groups[1].Value);

                return new SemanticVersion(majorVersion, minorVersion, patchVersion, null, 0);
            }
            catch (Exception)
            {
                return new SemanticVersion();
            }
        }

        internal static IEnumerable<KeyValuePair<string, string>> GetEditorPrefMigrationsForPlugin(Plugin p)
        {
            var fieldInfo = p.GetType().GetField("ID", BindingFlags.Public | BindingFlags.Static);
            var renamedFromAttributes = fieldInfo.GetCustomAttributes(typeof(RenamedFromAttribute), true)
                .Cast<RenamedFromAttribute>();
            foreach (var renamed in renamedFromAttributes)
            {
                foreach (var editorPref in p.configuration.editorPrefs)
                {
                    var previousKey = EditorPrefMetadata.GetNamespacedKey(renamed.previousName, editorPref.key);
                    if (EditorPrefs.HasKey(previousKey))
                    {
                        yield return new KeyValuePair<string, string>(previousKey, editorPref.namespacedKey);
                    }
                }
            }
        }

        internal static void MigrateEditorPref(string fromKey, string toKey)
        {
            if (!EditorPrefs.HasKey(fromKey))
                throw new InvalidOperationException($"No Editor Pref with key {fromKey} found, could not perform migration from {fromKey} to {toKey}");

            var value = new SerializationData(EditorPrefs.GetString(fromKey)).Deserialize();

            EditorPrefs.SetString(toKey, value.Serialize().json);
        }

        public static void MigrateEditorPreferences(Plugin p)
        {
            var editorPrefMigrations = GetEditorPrefMigrationsForPlugin(p);
            foreach (var migration in editorPrefMigrations)
            {
                MigrateEditorPref(migration.Key, migration.Value);
            }

            // Now that our editor prefs have been migrated on the machine, re-load our editor prefs to memory
            foreach (var editorPref in p.configuration.editorPrefs)
            {
                editorPref.Load();
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Migration_Asset_to_Package : PluginMigration
    {
        public Migration_Asset_to_Package(Plugin plugin) : base(plugin)
        {
            order = 1;
        }

        public override SemanticVersion @from => "1.4.1000";
        public override SemanticVersion to => "1.5.0-pre.0";

        public override void Run()
        {
            RemoveLegacyPackageFiles();

            // We need to clear our cached types so that legacy types (Bolt.x, Ludiq.y, etc) aren't held in memory
            // by name. When we deserialize our graphs anew, we need to deserialize them into their new types (with new
            // namespaces) and the cached type lookup will interfere with that. See RuntimeCodebase.TryDeserializeType()
            RuntimeCodebase.ClearCachedTypes();

            RuntimeCodebase.disallowedAssemblies.Add("Bolt.Core.Editor");
            RuntimeCodebase.disallowedAssemblies.Add("Bolt.Core.Runtime");
            RuntimeCodebase.disallowedAssemblies.Add("Bolt.Flow.Editor");
            RuntimeCodebase.disallowedAssemblies.Add("Bolt.Flow.Runtime");
            RuntimeCodebase.disallowedAssemblies.Add("Bolt.State.Editor");
            RuntimeCodebase.disallowedAssemblies.Add("Bolt.State.Runtime");
            RuntimeCodebase.disallowedAssemblies.Add("Ludiq.Core.Editor");
            RuntimeCodebase.disallowedAssemblies.Add("Ludiq.Core.Runtime");
            RuntimeCodebase.disallowedAssemblies.Add("Ludiq.Graphs.Editor");
            RuntimeCodebase.disallowedAssemblies.Add("Ludiq.Graphs.Runtime");

            ScriptReferenceResolver.Run();

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            plugin.configuration.Initialize();

            try
            {
                MigrateProjectSettings();
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                Debug.LogWarning("There was a problem migrating your Visual Scripting project settings. Be sure to check them in Edit -> Project Settings -> Visual Scripting");
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                Debug.LogError(e);
#endif
            }

            try
            {
                MigrationUtility_Asset_to_Package.MigrateEditorPreferences(this.plugin);
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                Debug.LogWarning("There was a problem migrating your Visual Scripting editor preferences. Be sure to check them in Edit -> Preferences -> Visual Scripting");
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                Debug.LogError(e);
#endif
            }

            try
            {
                MigrateVariablesAssets();
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                Debug.LogWarning("There was a problem migrating your Visual Scripting application or saved variables. You might want to restore your backup");
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                Debug.LogError(e);
#endif
            }
        }

        private static void RemoveLegacyPackageFiles()
        {
            // Todo: This partially fails because we can't delete the loaded sqlite3 dll.
            // Causes no problems for the migration here, but leaves files for the user to delete

            // Remove Assemblies
            var rootPath = MigrationUtility_Asset_to_Package.GetLegacyRootPath("Bolt.Core");
            var assembliesFullPath = $"{Directory.GetParent(rootPath).FullName}/Assemblies";
            var assembliesAssetPath = Path.Combine("Assets", PathUtility.FromAssets(assembliesFullPath));

            // Todo: This currently fails because of the sqlite dll. Deletes everything else
            AssetDatabase.DeleteAsset(assembliesAssetPath);

            // Remove icon map files
            AssetDatabase.DeleteAsset(MigrationUtility_Asset_to_Package.GetLegacyIconMapAssetPath("Bolt.Core"));
            AssetDatabase.DeleteAsset(MigrationUtility_Asset_to_Package.GetLegacyIconMapAssetPath("Bolt.Flow"));
            AssetDatabase.DeleteAsset(MigrationUtility_Asset_to_Package.GetLegacyIconMapAssetPath("Bolt.State"));
        }

        private static void MigrateProjectSettings()
        {
            // Merging Ludiq.Graphs, Ludiq.Core and Bolt.Core
            var legacyProjectSettingPluginIds = new string[]
                {"Ludiq.Graphs", "Ludiq.Core", "Bolt.Core"};

            BoltCore.Configuration.LoadOrCreateProjectSettingsAsset();

            foreach (var pluginId in legacyProjectSettingPluginIds)
            {
                var legacyProjectSettingsAsset = MigrationUtility_Asset_to_Package.GetLegacyProjectSettingsAsset(pluginId);
                if (legacyProjectSettingsAsset != null)
                {
                    BoltCore.Configuration.projectSettingsAsset.Merge(legacyProjectSettingsAsset);
                }
            }

            BoltCore.Configuration.SaveProjectSettingsAsset(true);
            BoltCore.Configuration.ResetProjectSettingsMetadata();
        }

        private static void MigrateVariablesAssets()
        {
            // We have application and saved variables to migrate
            var variableAssetNames = new string[] { "ApplicationVariables", "SavedVariables" };

            foreach (var fileName in variableAssetNames)
            {
                var rootPath = MigrationUtility_Asset_to_Package.GetLegacyRootPath("Bolt.Core");
                var legacyAssetPath = Path.Combine(rootPath, "Generated", "Variables", "Resources", fileName + ".asset");
                var newAssetPath = Path.Combine(Paths.assets, "Bolt.Generated", "VisualScripting.Core", "Variables", "Resources", fileName + ".asset");
                var directory = Path.GetDirectoryName(newAssetPath);

                if (File.Exists(legacyAssetPath) && !File.Exists(newAssetPath))
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.Copy(legacyAssetPath, newAssetPath);
                    File.Move(legacyAssetPath + ".meta", newAssetPath + ".meta");
                }
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Migration_Asset_to_Package_Post : PluginMigration
    {
        public Migration_Asset_to_Package_Post(Plugin plugin) : base(plugin)
        {
            order = 3;
        }

        public override SemanticVersion @from => "1.4.1000";
        public override SemanticVersion to => "1.5.0-pre.0";

        public override void Run()
        {
            CleanupLegacyUserFiles();

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private static void CleanupLegacyUserFiles()
        {
            // Todo: This partially fails because we can't delete the loaded sqlite3 dll.
            // Causes no problems for the migration here, but leaves files for the user to delete

            // Remove Old Ludiq folder, including project settings and unit database
            var rootPath = MigrationUtility_Asset_to_Package.GetLegacyRootPath("Bolt.Core");
            var ludiqFolderFullPath = Directory.GetParent(rootPath).FullName;
            var ludiqFolderAssetPath = Path.Combine("Assets", PathUtility.FromAssets(ludiqFolderFullPath));

            AssetDatabase.DeleteAsset(ludiqFolderAssetPath);
        }
    }

    [Plugin(BoltCore.ID)]
    internal class DeprecatedSavedVersionLoader_Bolt_AssetStore : PluginDeprecatedSavedVersionLoader
    {
        public DeprecatedSavedVersionLoader_Bolt_AssetStore(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.4.13";

        public override bool Run(out SemanticVersion savedVersion)
        {
            var manuallyParsedVersion = MigrationUtility_Asset_to_Package.TryManualParseSavedVersion("Bolt.Core");
            savedVersion = manuallyParsedVersion;

            return savedVersion != "0.0.0";
        }
    }
}
