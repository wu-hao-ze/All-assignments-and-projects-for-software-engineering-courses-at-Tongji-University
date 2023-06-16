using System;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class SavedVariables
    {
        #region Storage

        public const string assetPath = "SavedVariables";

        public const string playerPrefsKey = "LudiqSavedVariables";

        private static VariablesAsset _asset;

        public static VariablesAsset asset
        {
            get
            {
                if (_asset == null)
                {
                    Load();
                }

                return _asset;
            }
        }

        public static void Load()
        {
            _asset = Resources.Load<VariablesAsset>(assetPath) ?? ScriptableObject.CreateInstance<VariablesAsset>();
        }

        #endregion

        #region Lifecycle

        public static void OnEnterEditMode()
        {
            FetchSavedDeclarations();
            DestroyMergedDeclarations(); // Required because assemblies don't reload on play mode exit
        }

        public static void OnExitEditMode()
        {
            SaveDeclarations(saved);
        }

        internal static void OnEnterPlayMode()
        {
            FetchSavedDeclarations();
            MergeInitialAndSavedDeclarations();

            // The variables saver gameobject is only instantiated if its needed
            // It's only needed if a variable in our merged collection changes, requiring re-serialization as
            // the runtime ends
            merged.OnVariableChanged += () =>
            {
                if (VariablesSaver.instance == null)
                    VariablesSaver.Instantiate();
            };
        }

        internal static void OnExitPlayMode()
        {
            SaveDeclarations(merged);
        }

        #endregion

        #region Declarations

        public static VariableDeclarations initial => asset.declarations;

        public static VariableDeclarations saved { get; private set; }

        public static VariableDeclarations merged { get; private set; }

        public static VariableDeclarations current => Application.isPlaying ? merged : initial;

        public static void SaveDeclarations(VariableDeclarations declarations)
        {
            WarnAndNullifyUnityObjectReferences(declarations);

            try
            {
                var data = declarations.Serialize();

                if (data.objectReferences.Length != 0)
                {
                    // Hopefully, WarnAndNullify will have prevented this exception,
                    // but in case an object reference was nested as a member of the
                    // serialized objects, it wouldn't have caught it, and thus we need
                    // to abort the save process and inform the user.
                    throw new InvalidOperationException("Cannot use Unity object variable references in saved variables.");
                }

                PlayerPrefs.SetString(playerPrefsKey, data.json);
                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to save variables to player prefs: \n{ex}");
            }
        }

        public static void FetchSavedDeclarations()
        {
            if (PlayerPrefs.HasKey(playerPrefsKey))
            {
                try
                {
                    saved = (VariableDeclarations)new SerializationData(PlayerPrefs.GetString(playerPrefsKey)).Deserialize();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to fetch saved variables from player prefs: \n{ex}");
                    saved = new VariableDeclarations();
                }
            }
            else
            {
                saved = new VariableDeclarations();
            }
        }

        private static void MergeInitialAndSavedDeclarations()
        {
            merged = initial.CloneViaFakeSerialization();

            WarnAndNullifyUnityObjectReferences(merged);

            foreach (var name in saved.Select(vd => vd.name))
            {
                if (!merged.IsDefined(name))
                {
                    merged[name] = saved[name];
                }
                else if (merged[name] == null)
                {
                    if (saved[name] == null || saved[name].GetType().IsNullable())
                    {
                        merged[name] = saved[name];
                    }
                    else
                    {
                        Debug.LogWarning($"Cannot convert saved player pref '{name}' to null.\n");
                    }
                }
                else
                {
                    if (saved[name].IsConvertibleTo(merged[name].GetType(), true))
                    {
                        merged[name] = saved[name];
                    }
                    else
                    {
                        Debug.LogWarning($"Cannot convert saved player pref '{name}' to expected type ({merged[name].GetType()}).\nReverting to initial value.");
                    }
                }
            }
        }

        private static void DestroyMergedDeclarations()
        {
            merged = null;
        }

        private static void WarnAndNullifyUnityObjectReferences(VariableDeclarations declarations)
        {
            Ensure.That(nameof(declarations)).IsNotNull(declarations);

            foreach (var declaration in declarations)
            {
                if (declaration.value is UnityObject)
                {
                    Debug.LogWarning($"Saved variable '{declaration.name}' refers to a Unity object. This is not supported. Its value will be null.");
                    declarations[declaration.name] = null;
                }
            }
        }

        #endregion
    }
}
