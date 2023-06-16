using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class EditorUnityObjectUtility
    {
        static EditorUnityObjectUtility()
        {
            try
            {
                UnityTypeType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.UnityType", true);
                UnityTypeType_FindTypeByNameCaseInsensitive = UnityTypeType.GetMethod("FindTypeByNameCaseInsensitive", BindingFlags.Static | BindingFlags.Public);
                UnityTypeType_persistentTypeID = UnityTypeType.GetProperty("persistentTypeID", BindingFlags.Instance | BindingFlags.Public);

                if (UnityTypeType_FindTypeByNameCaseInsensitive == null)
                {
                    throw new MissingMemberException(UnityTypeType.ToString(), "FindTypeByNameCaseInsensitive");
                }

                if (UnityTypeType_persistentTypeID == null)
                {
                    throw new MissingMemberException(UnityTypeType.ToString(), "persistentTypeID");
                }

#if UNITY_2018_3_OR_NEWER
                {
                    // GetMethod doesn't work with generic methods...
                    var PrefabUtility_GetCorrespondingObjectFromSource_Candidates = typeof(PrefabUtility).GetMember("GetCorrespondingObjectFromSource", BindingFlags.Public | BindingFlags.Static);

                    if (PrefabUtility_GetCorrespondingObjectFromSource_Candidates.Length > 0)
                    {
                        PrefabUtility_GetCorrespondingObjectFromSource = ((MethodInfo)PrefabUtility_GetCorrespondingObjectFromSource_Candidates[0]).MakeGenericMethod(typeof(UnityObject));
                    }

                    PrefabUtility_GetPrefabInstanceHandle = typeof(PrefabUtility).GetMethod("GetPrefabInstanceHandle", BindingFlags.Public | BindingFlags.Static);
                    PrefabUtility_IsPartOfPrefabAsset = typeof(PrefabUtility).GetMethod("IsPartOfPrefabAsset", BindingFlags.Public | BindingFlags.Static);
                    PrefabUtility_IsPartOfPrefabInstance = typeof(PrefabUtility).GetMethod("IsPartOfPrefabInstance", BindingFlags.Public | BindingFlags.Static);
                    PrefabUtility_IsDisconnectedFromPrefabAsset = typeof(PrefabUtility).GetMethod("IsDisconnectedFromPrefabAsset", BindingFlags.Public | BindingFlags.Static);

                    if (PrefabUtility_GetCorrespondingObjectFromSource == null)
                    {
                        throw new MissingMemberException(typeof(PrefabUtility).ToString(), "GetCorrespondingObjectFromSource");
                    }

                    if (PrefabUtility_GetPrefabInstanceHandle == null)
                    {
                        throw new MissingMemberException(typeof(PrefabUtility).ToString(), "GetPrefabInstanceHandle");
                    }

                    if (PrefabUtility_IsPartOfPrefabAsset == null)
                    {
                        throw new MissingMemberException(typeof(PrefabUtility).ToString(), "IsPartOfPrefabAsset");
                    }

                    if (PrefabUtility_IsPartOfPrefabInstance == null)
                    {
                        throw new MissingMemberException(typeof(PrefabUtility).ToString(), "IsPartOfPrefabInstance");
                    }

                    if (PrefabUtility_IsDisconnectedFromPrefabAsset == null)
                    {
                        throw new MissingMemberException(typeof(PrefabUtility).ToString(), "IsDisconnectedFromPrefabAsset");
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
        }

        public static IEnumerable<Type> GetUnityTypes(UnityObject target)
        {
            Ensure.That(nameof(target)).IsNotNull(target);

            if (target.IsComponentHolder())
            {
                yield return typeof(GameObject);

                foreach (var componentType in target.GetComponents<Component>().NotNull().Select(c => c.GetType()).Distinct())
                {
                    yield return componentType;
                }
            }
            else
            {
                yield return target.GetType();
            }
        }

        #region Prefabs

#if UNITY_2018_3_OR_NEWER
        // public class UnityEditor.SceneManagement.PrefabStageUtility (or UnityEditor.Experimental.SceneManagement.PrefabStageUtility if < 2021.2)
        private static readonly Type PrefabStageUtilityType; // public class UnityEditor.Experimental.SceneManagement.PrefabStageUtility
        private static readonly MethodInfo PrefabStageUtility_GetPrefabStage;
        private static readonly MethodInfo PrefabUtility_GetCorrespondingObjectFromSource;
        private static readonly MethodInfo PrefabUtility_GetPrefabInstanceHandle;
        private static readonly MethodInfo PrefabUtility_IsPartOfPrefabAsset;
        private static readonly MethodInfo PrefabUtility_IsPartOfPrefabInstance;
        private static readonly MethodInfo PrefabUtility_IsDisconnectedFromPrefabAsset;
#endif

        public static UnityObject GetPrefabDefinition(this UnityObject uo)
        {
            Ensure.That(nameof(uo)).IsNotNull(uo);

#if UNITY_2018_3_OR_NEWER
            try
            {
                return (UnityObject)PrefabUtility_GetCorrespondingObjectFromSource.InvokeOptimized(null, uo);
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
#else
            return PrefabUtility.GetPrefabParent(uo);
#endif
        }

        private static UnityObject GetPrefabInstance(this UnityObject uo)
        {
#if UNITY_2018_3_OR_NEWER
            try
            {
                return (UnityObject)PrefabUtility_GetPrefabInstanceHandle.InvokeOptimized(null, uo);
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
#else
            return PrefabUtility.GetPrefabObject(uo);
#endif
        }

        public static bool IsPrefabInstance(this UnityObject uo)
        {
            Ensure.That(nameof(uo)).IsNotNull(uo);

#if UNITY_2018_3_OR_NEWER
            try
            {
                return (bool)PrefabUtility_IsPartOfPrefabInstance.InvokeOptimized(null, uo);
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }

#else
            return GetPrefabDefinition(uo) != null;
#endif
        }

        public static bool IsPrefabDefinition(this UnityObject uo)
        {
            Ensure.That(nameof(uo)).IsNotNull(uo);

#if UNITY_2018_3_OR_NEWER
            try
            {
                // https://forum.unity.com/threads/editorgui-objectfield-allowsceneobjects-in-isolation-mode.610564/
                return (bool)PrefabUtility_IsPartOfPrefabAsset.InvokeOptimized(null, uo);
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }

#else
            return GetPrefabDefinition(uo) == null && GetPrefabInstance(uo) != null;
#endif
        }

        public static bool IsConnectedPrefabInstance(this UnityObject uo)
        {
            Ensure.That(nameof(uo)).IsNotNull(uo);

#if UNITY_2018_3_OR_NEWER
            try
            {
                return
                    (bool)PrefabUtility_IsPartOfPrefabInstance.InvokeOptimized(null, uo) &&
                    !(bool)PrefabUtility_IsDisconnectedFromPrefabAsset.InvokeOptimized(null, uo);
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }

#else
            return IsPrefabInstance(uo) && GetPrefabInstance(uo) != null;
#endif
        }

        public static bool IsDisconnectedPrefabInstance(this UnityObject uo)
        {
            Ensure.That(nameof(uo)).IsNotNull(uo);

#if UNITY_2018_3_OR_NEWER
            try
            {
                return
                    (bool)PrefabUtility_IsPartOfPrefabInstance.InvokeOptimized(null, uo) &&
                    (bool)PrefabUtility_IsDisconnectedFromPrefabAsset.InvokeOptimized(null, uo);
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }

#else
            return IsPrefabInstance(uo) && GetPrefabInstance(uo) == null;
#endif
        }

        public static bool IsSceneBound(this UnityObject uo)
        {
            Ensure.That(nameof(uo)).IsNotNull(uo);

#if UNITY_2018_3_OR_NEWER
            return
                (uo is GameObject go && !IsPrefabDefinition(go)) ||
                (uo is Component component && !IsPrefabDefinition(component.gameObject));
#else
            return
                (uo is GameObject go && !IsPrefabDefinition(go)) ||
                (uo is Component component && !IsPrefabDefinition(component.gameObject));
#endif
        }

        #endregion


        #region Class

        public const int MonoBehaviourClassID = 114;
        private static readonly Type UnityTypeType; // internal sealed class UnityType
        private static readonly PropertyInfo UnityTypeType_persistentTypeID; // public int persistentTypeID { get; private set; }
        private static readonly MethodInfo UnityTypeType_FindTypeByNameCaseInsensitive; // public static extern int StringToClassIDCaseInsensitive(string classString);

        public static int GetClassID(Type type)
        {
            if (typeof(MonoBehaviour).IsAssignableFrom(type) || typeof(ScriptableObject).IsAssignableFrom(type))
            {
                return MonoBehaviourClassID;
            }

            try
            {
                var unityType = UnityTypeType_FindTypeByNameCaseInsensitive.Invoke(null, new object[] { type.Name });

                if (unityType == null)
                {
                    throw new Exception($"Could not find UnityType for '{type}'.");
                }

                return (int)UnityTypeType_persistentTypeID.GetValue(unityType, null);
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
        }

        public static string GetScriptClass(Type type)
        {
            if (!typeof(MonoBehaviour).IsAssignableFrom(type) && !typeof(ScriptableObject).IsAssignableFrom(type))
            {
                throw new NotSupportedException("Trying to get script class of a non-script type.");
            }

            return type.Name;
        }

        #endregion
    }
}
