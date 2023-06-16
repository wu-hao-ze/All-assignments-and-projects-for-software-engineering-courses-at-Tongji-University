using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting.Analytics;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class VSMigrationUtility
    {
        private readonly List<Plugin> plugins;
        private readonly List<MigrationStep> steps;
        private MigrationAnalytics.Data analyticsData;

        public VSMigrationUtility()
        {
            IEnumerable<Plugin> allPlugins = PluginContainer.GetAllPlugins();

            plugins = allPlugins.OrderByDependencies().ToList();

            foreach (var plugin in plugins)
            {
                plugin.resources.LoadMigrations();
            }

            steps = this.plugins
                .SelectMany(plugin =>
                    plugin.resources.pendingMigrations.Select(migration => new MigrationStep(plugin, migration)))
                .OrderBy(step => step.migration.from)
                .ThenBy(step => step.migration.order)
                .ToList();

            analyticsData = new MigrationAnalytics.Data
            {
                total = new MigrationAnalytics.MigrationStepAnalyticsData()
                {
                    from = BoltCore.Manifest.savedVersion.ToString(),
                    to = BoltCore.Manifest.currentVersion.ToString(),
                    pluginId = "VS",
                    success = true
                },
                steps = new List<MigrationAnalytics.MigrationStepAnalyticsData>()
            };
            foreach (var step in steps)
            {
                analyticsData.steps.Add(new MigrationAnalytics.MigrationStepAnalyticsData()
                {
                    from = step.migration.from.ToString(),
                    to = step.migration.to.ToString(),
                    pluginId = step.plugin.id,
                });
            }
        }

        public void OnUpdate()
        {
            // If there are no migration steps required, don't bother the user
            if (steps.Count <= 0)
            {
                SetPluginVersionsToCurrent();
                MigrationAnalytics.Send(analyticsData);
                return;
            }

            var firstVSPackageVersion = new SemanticVersion(1, 5, 0, "", 0);
            var olderVersionText = BoltCore.Manifest.savedVersion < firstVSPackageVersion ? "Unity Visual Scripting (Bolt)" : "Unity Visual Scripting";

            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                EditorUtility.DisplayDialog("Unity Visual Scripting Upgrade",
                    $"We've detected an older version of {olderVersionText}.\n\n" +
                    "We can't migrate your project unless you use ForceText as your serialization mode. Go to Edit -> Project Settings -> Editor -> Asset Serialization -> Mode to set it.\n\n" +
                    "Re-initiate the migration by installing the package.",
                    "OK / Uninstall");

                Client.Remove("com.unity.visualscripting");
                return;
            }

            var ok = EditorUtility.DisplayDialog("Unity Visual Scripting Upgrade",
                $"We've detected an older version of {olderVersionText}.\n\n" +
                "Your project and bolt assets will be backed up and migrated to work with the newest version. This can take a few minutes.",
                "Migrate My Project", "Cancel / Uninstall");

            if (!ok)
            {
                Client.Remove("com.unity.visualscripting");
                return;
            }

            VSBackupUtility.Backup();

            // ClearLog();

            for (var i = 0; i < steps.Count; ++i)
            {
                var step = steps[i];
                step.Reset();
                step.Run();

                if (step.state == MigrationStep.State.Failure)
                {
                    Debug.LogWarning(
                        $"VisualScripting - A migration step for {step.plugin.id} failed! Your project might be in an invalid state, restore your backup and try again...");
                    analyticsData.steps[i].success = false;
                    analyticsData.steps[i].exception = AnalyticsUtilities.AnonymizeException(step.exception);
                    analyticsData.total.success = false;
                    break;
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                    throw step.exception;
#endif
                }
                else
                {
                    analyticsData.steps[i].success = true;
                }
            }

            Complete();
        }

        protected void Complete()
        {
            // Make sure all plugins are set to their latest version, even if they
            // don't have a migration to it.
            SetPluginVersionsToCurrent();

            AssetDatabase.SaveAssets();

            MigrationAnalytics.Send(analyticsData);

            var ok = EditorUtility.DisplayDialog("Unity Visual Scripting Upgrade",
                "Migration complete!", "OK");

            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        private void SetPluginVersionsToCurrent()
        {
            foreach (var plugin in plugins)
            {
                plugin.manifest.savedVersion = plugin.manifest.currentVersion;
                plugin.configuration.SaveProjectSettingsAsset(true);
            }
        }

        private static void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        internal class MigrationStep
        {
            public enum State
            {
                Idle,
                Migrating,
                Success,
                Failure
            }

            public MigrationStep(Plugin plugin, PluginMigration migration)
            {
                this.plugin = plugin;
                this.migration = migration;
            }

            internal readonly Plugin plugin;
            internal readonly PluginMigration migration;
            internal Exception exception;

            public State state { get; private set; }

            private EditorTexture GetStateIcon(State state)
            {
                switch (state)
                {
                    case State.Idle:
                        return BoltCore.Icons.empty;
                    case State.Migrating:
                        return BoltCore.Icons.progress;
                    case State.Success:
                        return BoltCore.Icons.successState;
                    case State.Failure:
                        return BoltCore.Icons.errorState;
                    default:
                        throw new UnexpectedEnumValueException<State>(state);
                }
            }

            public void Run()
            {
                state = State.Migrating;
                try
                {
                    migration.Run();
                    exception = null;
                    state = State.Success;
                    plugin.manifest.savedVersion = migration.to;
                    InternalEditorUtility.RepaintAllViews();
                }
                catch (Exception ex)
                {
                    state = State.Failure;
                    exception = ex;
                }
            }

            public void Reset()
            {
                state = State.Idle;
                exception = null;
            }
        }
    }
}
