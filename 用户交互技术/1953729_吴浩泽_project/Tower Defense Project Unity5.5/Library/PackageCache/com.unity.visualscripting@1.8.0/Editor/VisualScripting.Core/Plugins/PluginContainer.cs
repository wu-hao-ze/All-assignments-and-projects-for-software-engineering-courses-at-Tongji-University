using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.MPE;
#endif

namespace Unity.VisualScripting
{
    public sealed class PluginContainer
    {
        internal static void InitializeOnLoad()
        {
            // Fixes console errors shown in Standalone Profiler window (Bolt-1289).
            // Note: MPE as a whole (including Standalone Profiler) is going away, will need to remove this
            // when it does. See: https://unity.slack.com/archives/CHVTMBEF5/p1613683381195300
#if UNITY_2020_2_OR_NEWER
#if UNITY_2021_1_OR_NEWER
            if (ProcessService.level != ProcessLevel.Main)
#else
            if (ProcessService.level != ProcessLevel.Master)
#endif
                return;
#endif

            Initialize();
        }

        private static bool initializing;

        private static Dictionary<string, Plugin> pluginsById;

        private static Dictionary<string, Type> pluginTypesById;

        internal static Dictionary<string, HashSet<string>> pluginDependencies;

        private static readonly ConcurrentQueue<Action> delayQueue = new ConcurrentQueue<Action>();

        public static event Action delayCall
        {
            add
            {
                Ensure.That(nameof(value)).IsNotNull(value);

                if (initialized)
                {
                    value.Invoke();
                }
                else
                {
                    delayQueue.Enqueue(value);
                }
            }
            remove { }
        }

        public static bool initialized { get; private set; }

        public static IEnumerable<Plugin> plugins
        {
            get
            {
                EnsureInitialized();

                return pluginsById.Values;
            }
        }

        public static void UpdateVersionMismatch()
        {
            anyVersionMismatch = plugins.Any(p => p.manifest.versionMismatch);
        }

        internal static void Initialize()
        {
            using (ProfilingUtility.SampleBlock("Plugin Container Initialization"))
            {
                initializing = true;

                pluginTypesById = Codebase.ludiqEditorTypes
                    .Where(t => typeof(Plugin).IsAssignableFrom(t) && t.IsConcrete())
                    .ToDictionary(GetPluginID);

                pluginDependencies = new Dictionary<string, HashSet<string>>();

                foreach (var pluginTypeById in pluginTypesById)
                {
                    pluginDependencies.Add(pluginTypeById.Key, pluginTypeById.Value.GetAttributes<PluginDependencyAttribute>().Select(pda => pda.id).ToHashSet());
                }

                var moduleTypes = Codebase.ludiqEditorTypes
                    .Where(t => typeof(IPluginModule).IsAssignableFrom(t) && t.HasAttribute<PluginModuleAttribute>(false))
                    .OrderByDependencies(t => t.GetAttributes<PluginModuleDependencyAttribute>().Select(pmda => pmda.moduleType))
                    .ToArray();

                pluginsById = new Dictionary<string, Plugin>();

                var allModules = new List<IPluginModule>();

                foreach (var pluginId in pluginTypesById.Keys.OrderByDependencies(pluginId => pluginDependencies[pluginId]))
                {
                    var pluginType = pluginTypesById[pluginId];

                    Plugin plugin;

                    try
                    {
                        using (ProfilingUtility.SampleBlock($"{pluginType.Name} (Instantiation)"))
                        {
                            plugin = (Plugin)pluginType.Instantiate();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new TargetInvocationException($"Could not instantiate plugin '{pluginId}' ('{pluginType.CSharpName()}').", ex);
                    }

                    var modules = new List<IPluginModule>();

                    foreach (var moduleType in moduleTypes)
                    {
                        try
                        {
                            var required = moduleType.GetAttribute<PluginModuleAttribute>(false).required;

                            var moduleProperty = pluginType.GetProperties().FirstOrDefault(p => p.PropertyType.IsAssignableFrom(moduleType));

                            if (moduleProperty == null)
                            {
                                continue;
                            }

                            IPluginModule module = null;

                            var moduleOverrideType = Codebase.ludiqEditorTypes
                                .FirstOrDefault(t => moduleType.IsAssignableFrom(t) && t.IsConcrete() && t.HasAttribute<PluginAttribute>() && t.GetAttribute<PluginAttribute>().id == pluginId);

                            if (moduleOverrideType != null)
                            {
                                try
                                {
                                    using (ProfilingUtility.SampleBlock($"{moduleOverrideType.Name} (Instantiation)"))
                                    {
                                        module = (IPluginModule)InstantiateLinkedType(moduleOverrideType, plugin);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new TargetInvocationException($"Failed to instantiate user-defined plugin module '{moduleOverrideType.CSharpName()}' for '{pluginId}'.", ex);
                                }
                            }
                            else if (moduleType.IsConcrete())
                            {
                                try
                                {
                                    using (ProfilingUtility.SampleBlock($"{moduleType.Name} (Instantiation)"))
                                    {
                                        module = (IPluginModule)InstantiateLinkedType(moduleType, plugin);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new TargetInvocationException($"Failed to instantiate built-in plugin module '{moduleType.CSharpName()}' for '{pluginId}'.", ex);
                                }
                            }
                            else if (required)
                            {
                                throw new InvalidImplementationException($"Missing implementation of plugin module '{moduleType.CSharpName()}' for '{pluginId}'.");
                            }

                            if (module != null)
                            {
                                moduleProperty.SetValue(plugin, module, null);

                                modules.Add(module);
                                allModules.Add(module);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }

                    pluginsById.Add(plugin.id, plugin);

                    foreach (var module in modules)
                    {
                        try
                        {
                            using (ProfilingUtility.SampleBlock($"{module.GetType().Name} (Initialization)"))
                            {
                                module.Initialize();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(new Exception($"Failed to initialize plugin module '{plugin.id}.{module.GetType().CSharpName()}'.", ex));
                        }
                    }

                    if (plugin.manifest.versionMismatch)
                    {
                        anyVersionMismatch = true;
                    }
                }

                foreach (var module in allModules)
                {
                    try
                    {
                        using (ProfilingUtility.SampleBlock($"{module.GetType().Name} (Late Initialization)"))
                        {
                            module.LateInitialize();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(new Exception($"Failed to late initialize plugin module '{module.plugin.id}.{module.GetType().CSharpName()}'.", ex));
                    }
                }

                var afterPluginTypes = Codebase.ludiqEditorTypes
                    .Where(t => t.HasAttribute<InitializeAfterPluginsAttribute>());

                using (ProfilingUtility.SampleBlock($"BeforeInitializeAfterPlugins"))
                {
                    EditorApplicationUtility.BeforeInitializeAfterPlugins();
                }

                foreach (var afterPluginType in afterPluginTypes)
                {
                    using (ProfilingUtility.SampleBlock($"{afterPluginType.Name} (Static Initializer)"))
                    {
                        RuntimeHelpers.RunClassConstructor(afterPluginType.TypeHandle);
                    }
                }

                using (ProfilingUtility.SampleBlock($"AfterInitializeAfterPlugins"))
                {
                    EditorApplicationUtility.AfterInitializeAfterPlugins();
                }

                using (ProfilingUtility.SampleBlock($"Delayed Calls"))
                {
                    while (delayQueue.TryDequeue(out var a))
                    {
                        a.Invoke();
                    }
                }

                InternalEditorUtility.RepaintAllViews();

                ProfilingUtility.Clear();

                using (ProfilingUtility.SampleBlock($"Product Container Initialization"))
                {
                    ProductContainer.Initialize();
                }

                initializing = false;

                initialized = true;

                using (ProfilingUtility.SampleBlock($"Update Process"))
                {
                    // Automatically show update wizard

                    if (!EditorApplication.isPlayingOrWillChangePlaymode && plugins.Any(plugin => plugin.manifest.versionMismatch))
                    {
                        // Delay call seems to be needed here to avoid arcane exceptions...
                        // Too lazy to debug why, it works that way.
                        EditorApplication.delayCall += PerformUpdate;
                    }
                }
            }
        }

        private static void PerformUpdate()
        {
            if (plugins.Any(plugin => plugin.manifest.savedVersion != plugin.manifest.currentVersion))
            {
#if VISUAL_SCRIPT_DEBUG_MIGRATION
                foreach (var plugin in plugins)
                {
                    Debug.Log($"plugin {plugin.id} saved version is {plugin.manifest.savedVersion} compared to current {plugin.manifest.currentVersion}");
                }
#endif
                (new VSMigrationUtility()).OnUpdate();
            }
        }

        public static void ImportUnits()
        {
            if (initialized)
            {
                foreach (Product product in ProductContainer.products)
                {
                    IEnumerable<Plugin> productPlugins = product.plugins.ResolveDependencies();

                    foreach (Plugin plugin in productPlugins)
                    {
                        if (plugin.id == "VisualScripting.Flow")
                        {
                            plugin.RunAction();

                            break;
                        }
                    }
                }
            }
        }

        private static void EnsureInitialized()
        {
            if (initializing)
            {
                return;
            }

            if (!initialized)
            {
                throw new InvalidOperationException("Trying to access plugin container before it is initialized.");
            }
        }

        public static string GetPluginID(Type linkedType)
        {
            Ensure.That(nameof(linkedType)).IsNotNull(linkedType);
            Ensure.That(nameof(linkedType)).HasAttribute<PluginAttribute>(linkedType);

            return linkedType.GetAttribute<PluginAttribute>().id;
        }

        public static Type GetPluginType(string pluginId)
        {
            EnsureInitialized();

            Ensure.That(nameof(pluginId)).IsNotNull(pluginId);

            return pluginTypesById[pluginId];
        }

        public static IEnumerable<Plugin> GetAllPlugins()
        {
            EnsureInitialized();

            return pluginsById.Values;
        }

        public static Plugin GetPlugin(string pluginId)
        {
            EnsureInitialized();

            Ensure.That(nameof(pluginId)).IsNotNull(pluginId);

            return pluginsById[pluginId];
        }

        public static bool HasPlugin(string pluginId)
        {
            EnsureInitialized();

            Ensure.That(nameof(pluginId)).IsNotNull(pluginId);

            return pluginsById.ContainsKey(pluginId);
        }

        private static IPluginLinked InstantiateLinkedType(Type linkedType, Plugin plugin)
        {
            Ensure.That(nameof(linkedType)).IsNotNull(linkedType);
            Ensure.That(nameof(plugin)).IsNotNull(plugin);

            return (IPluginLinked)linkedType.GetConstructorAccepting(plugin.GetType()).Invoke(new object[] { plugin });
        }

        internal static IEnumerable<Type> GetLinkedTypes(Type linkedType, string pluginId)
        {
            Ensure.That(nameof(linkedType)).IsNotNull(linkedType);

            return Codebase.ludiqEditorTypes.Where(t => linkedType.IsAssignableFrom(t) && t.IsConcrete() && t.HasAttribute<PluginAttribute>() && t.GetAttribute<PluginAttribute>().id == pluginId);
        }

        internal static IPluginLinked[] InstantiateLinkedTypes(Type linkedType, Plugin plugin)
        {
            Ensure.That(nameof(linkedType)).IsNotNull(linkedType);
            Ensure.That(nameof(plugin)).IsNotNull(plugin);

            return GetLinkedTypes(linkedType, plugin.id).Select(t => InstantiateLinkedType(t, plugin)).ToArray();
        }

        public static bool anyVersionMismatch { get; private set; }
    }
}
