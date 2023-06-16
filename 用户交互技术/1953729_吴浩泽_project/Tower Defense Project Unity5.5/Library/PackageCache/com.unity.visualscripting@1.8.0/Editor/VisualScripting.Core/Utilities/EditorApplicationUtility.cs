using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class EditorApplicationUtility
    {
        internal static void Initialize()
        {
            Recursion.safeMode = true;
            OptimizedReflection.safeMode = true;
            Ensure.IsActive = true;

            Selection.selectionChanged += OnSelectionChange;

#if UNITY_2018_1_OR_NEWER
            EditorApplication.projectChanged += OnProjectChange;
#else
            EditorApplication.projectWindowChanged += OnProjectChange;
#endif

#if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged += OnHierarchyChange;
#else
            EditorApplication.hierarchyWindowChanged += OnHierarchyChange;
#endif

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        #region Version

        private static SemanticVersion? _unityVersion;

        private static readonly SemanticVersion fallbackUnityVersion = "2017.4.0";

        public static SemanticVersion unityVersion
        {
            get
            {
                if (_unityVersion == null)
                {
                    var unityVersionString = Application.unityVersion;

                    if (SemanticVersion.TryParse(unityVersionString, out var parsedUnityVersion))
                    {
                        _unityVersion = parsedUnityVersion;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to parse Unity version string '{unityVersionString}', falling back to {fallbackUnityVersion}");
                        _unityVersion = fallbackUnityVersion;
                    }
                }

                return _unityVersion.Value;
            }
        }

        #endregion


        #region Assembly Lock

        public static bool isAssemblyReloadLocked { get; private set; }

        private static bool wantedScriptChangesDuringPlay;

        public static void LockReloadAssemblies()
        {
            isAssemblyReloadLocked = true;
            EditorApplication.LockReloadAssemblies();
        }

        public static void UnlockReloadAssemblies()
        {
            EditorApplication.UnlockReloadAssemblies();
            isAssemblyReloadLocked = false;
        }

#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Force Unlock Assembly Reload", priority = LudiqProduct.DeveloperToolsMenuPriority + 601)]
#endif
        public static void ClearProgressBar()
        {
            EditorApplication.UnlockReloadAssemblies();
            isAssemblyReloadLocked = false;
        }

        public static bool WantsScriptChangesDuringPlay()
        {
            return EditorPrefs.GetInt("ScriptCompilationDuringPlay", 0) == 0;
        }

        #endregion


        #region Events

        public static event Action onAssemblyReload;

        public static event Action onEnterPlayMode;

        public static event Action onExitPlayMode;

        public static event Action onEnterEditMode;

        public static event Action onExitEditMode;

        public static event Action onModeChange;

        public static event Action onPause;

        public static event Action onResume;

        public static event Action onPauseChange;

        public static event Action onSelectionChange;

        public static event Action onProjectChange;

        public static event Action onHierarchyChange;

        public static event Action onUndoRedo;

        private static void OnSelectionChange()
        {
            if (PluginContainer.initialized)
            {
                if (!VSUsageUtility.isVisualScriptingUsed) return;

                LudiqGUIUtility.BeginNotActuallyOnGUI();
                onSelectionChange?.Invoke();
                LudiqGUIUtility.EndNotActuallyOnGUI();
            }
        }

        private static void OnProjectChange()
        {
            if (PluginContainer.initialized)
            {
                if (!VSUsageUtility.isVisualScriptingUsed) return;

                LudiqGUIUtility.BeginNotActuallyOnGUI();
                onProjectChange?.Invoke();
                LudiqGUIUtility.EndNotActuallyOnGUI();
            }
        }

        private static void OnHierarchyChange()
        {
            if (PluginContainer.initialized)
            {
                if (!VSUsageUtility.isVisualScriptingUsed) return;

                LudiqGUIUtility.BeginNotActuallyOnGUI();
                onHierarchyChange?.Invoke();
                LudiqGUIUtility.EndNotActuallyOnGUI();
            }
        }

        private static void OnUndoRedo()
        {
            if (PluginContainer.initialized)
            {
                if (!VSUsageUtility.isVisualScriptingUsed) return;

                LudiqGUIUtility.BeginNotActuallyOnGUI();
                onUndoRedo?.Invoke();
                LudiqGUIUtility.EndNotActuallyOnGUI();
            }
        }

        private static void OnPause()
        {
            if (!VSUsageUtility.isVisualScriptingUsed) return;

            onPause?.Invoke();
        }

        private static void OnResume()
        {
            if (!VSUsageUtility.isVisualScriptingUsed) return;

            onResume?.Invoke();
        }

        private static void OnPauseChange()
        {
            if (!VSUsageUtility.isVisualScriptingUsed) return;

            onPauseChange?.Invoke();
        }

        private static void OnEnteredEditMode()
        {
            if (!VSUsageUtility.isVisualScriptingUsed) return;

            if (wantedScriptChangesDuringPlay)
            {
                UnlockReloadAssemblies();
                AssetDatabase.Refresh();
            }

            onEnterEditMode?.Invoke();
        }

        private static void OnExitingEditMode()
        {
            if (!VSUsageUtility.isVisualScriptingUsed) return;

            onExitEditMode?.Invoke();
        }

        private static void OnEnteredPlayMode()
        {
            if (!VSUsageUtility.isVisualScriptingUsed) return;

            onEnterPlayMode?.Invoke();

            wantedScriptChangesDuringPlay = WantsScriptChangesDuringPlay();

            if (wantedScriptChangesDuringPlay)
            {
                LockReloadAssemblies();
            }
        }

        private static void OnExitingPlayMode()
        {
            if (!VSUsageUtility.isVisualScriptingUsed) return;

            onExitPlayMode?.Invoke();
        }

        private static void OnModeChange()
        {
            if (!VSUsageUtility.isVisualScriptingUsed) return;

            onModeChange?.Invoke();
        }

        private static void OnAssemblyReload()
        {
            if (!VSUsageUtility.isVisualScriptingUsed) return;

            onAssemblyReload?.Invoke();
        }

        internal static void BeforeInitializeAfterPlugins()
        {
            EditorApplication.pauseStateChanged += delegate (PauseState pauseState)
            {
                switch (pauseState)
                {
                    case PauseState.Paused:
                        OnPause();
                        break;
                    case PauseState.Unpaused:
                        OnResume();
                        break;
                }

                OnPauseChange();
            };

            EditorApplication.playModeStateChanged += delegate (PlayModeStateChange stateChange)
            {
                switch (stateChange)
                {
                    case PlayModeStateChange.EnteredEditMode:
                        OnEnteredEditMode();
                        break;
                    case PlayModeStateChange.ExitingEditMode:
                        OnExitingEditMode();
                        break;
                    case PlayModeStateChange.EnteredPlayMode:
                        OnEnteredPlayMode();
                        break;
                    case PlayModeStateChange.ExitingPlayMode:
                        OnExitingPlayMode();
                        break;
                }

                OnModeChange();
            };
        }

        internal static void AfterInitializeAfterPlugins()
        {
            OnAssemblyReload();

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // Playmode state changed does not get called when
                // the editor assemblies load, therefore we have to
                // manually invoke enter edit mode.

                // This won't cause a double invoke because oddly,
                // assemblies do not get reloaded when you exit
                // play mode and get back into the edit mode. This may
                // cause issues somewhere in the Ludiq / Bolt source,
                // because most of it was coded assuming that they did.
                OnEnteredEditMode();
            }
        }

        #endregion
    }
}
