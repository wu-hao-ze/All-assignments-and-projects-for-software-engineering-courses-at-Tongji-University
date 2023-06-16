using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    // Important, see static constructor below for more details
    [InitializeOnLoad]
    public static class VSUsageUtility
    {
        /* Need to be careful with this constructor:
         * A major difference between Visual Scripting and other packages is that VS is installed by default in all projects
         * This makes it discoverable, but also means that potentially all users pay the cost of our background processes
         *
         * This utility let's us isolate users who *actually* are using Visual Scripting from users who aren't.
         * For those who aren't, we want to minimize the cost they pay. InitializeOnLoad triggers on every domain reload
         * so we need to be careful with what we do in this constructor and cache what we can to avoid calculation.
         */
        static VSUsageUtility()
        {
            CheckAndSetIsVisualScriptingUsed();
            DoInitializeOnLoadCalls();
        }

        private static void CheckAndSetIsVisualScriptingUsed()
        {
            // Using the SessionState API, this lets us cache our boolean persisted through domain reloads, but cleared
            // on Unity exit
            var sessionStateSet = SessionState.GetBool("Unity.VisualScripting.IsVisualScriptingUsedSet", false);
            if (sessionStateSet)
            {
                isVisualScriptingUsed = SessionState.GetBool("Unity.VisualScripting.IsVisualScriptingUsed", false);
                return;
            }

            // Everything under here is only calculated once per Unity session start (project open)
            var isVSUsed = false;

            // If we want to check on any state once per editor start-up that tells us if VS is being used, do that here

            // in batchmode or when someone builds a project using vs without opening a graph themselves, we still need
            // to consider it used to ensure the AOTPrebuilder/link.xml generation still kicks in
            isVSUsed = AssetDatabase.IsValidFolder(PluginPaths.ASSETS_FOLDER_BOLT_GENERATED);

            isVSUsed = isVSUsed || IsOlderVersionOfVSUsed();

            isVisualScriptingUsed = isVSUsed;
        }

        // Note: This is currently only possible because all our InitializeOnLoad calls were in Core. If we ever
        // need to put something in Flow or State, we might need to move this entire class to a shared assembly that
        // references those assemblies.
        private static void DoInitializeOnLoadCalls()
        {
            if (!isVisualScriptingUsed)
                return;

            if (_initializeOnLoadExecuted)
                return;

            _initializeOnLoadExecuted = true;

            EditorDebugUtility.DeleteDebugLogFile();

            EditorPlatformUtility.InitializeActiveBuildTarget();

            EditorApplicationUtility.Initialize();

            EditorTimeUtility.Initialize();

            UnityAPI.Initialize();

            PackageEventListener.SubscribeToEvents();

            PluginContainer.InitializeOnLoad();

            XmlDocumentation.Initialize();
        }

        private static bool IsOlderVersionOfVSUsed()
        {
            // Instantiating our deprecated loaders explicitly to avoid plugin container / plugin initialization logic
            // Is ordered in descending order (newest versions first)
            var deprecatedLoaders = new PluginDeprecatedSavedVersionLoader[]
            {
                new DeprecatedSavedVersionLoader_1_6_1(null),
                new DeprecatedSavedVersionLoader_1_5_1(null),
                new DeprecatedSavedVersionLoader_Bolt_AssetStore(null)
            };

            // We don't need to use the old loaders for every plugin, just core, because we just want to know *if*
            // an older version is being used or not
            return deprecatedLoaders.Select(savedVersionLoader => savedVersionLoader.Run(out var loadedVersion))
                                    .Any(success => success);
        }

        public static bool isVisualScriptingUsed
        {
            get => _isVisualScriptingUsed;
            set
            {
                SessionState.SetBool("Unity.VisualScripting.IsVisualScriptingUsedSet", true);
                SessionState.SetBool("Unity.VisualScripting.IsVisualScriptingUsed", value);

                if (_isVisualScriptingUsed == value)
                    return;

                _isVisualScriptingUsed = value;

                // The first time we determine that Visual Scripting is being used, we trigger other things
                if (_isVisualScriptingUsed)
                {
                    DoInitializeOnLoadCalls();

                    // Ensure our savedVersion is serialized for future migrations
                    BoltCore.Manifest.savedVersion = BoltCore.Manifest.savedVersion;

                    // Ensure that our nodes / units are imported and cached
                    PluginContainer.ImportUnits();
                }
            }
        }
        private static bool _isVisualScriptingUsed = false;

        // Used to protect against doing the initialize on load calls twice in the same app domain frame.
        // This can only happen on editor start, if we set _isVisualScriptingUsed to true
        private static bool _initializeOnLoadExecuted = false;
    }
}
