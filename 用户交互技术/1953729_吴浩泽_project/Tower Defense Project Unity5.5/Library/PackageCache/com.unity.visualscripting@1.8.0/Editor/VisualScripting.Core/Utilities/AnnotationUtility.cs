using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class AnnotationUtility
    {
        static AnnotationUtility()
        {
            try
            {
                AnnotationUtilityType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.AnnotationUtility", true);
                AnnotationUtility_GetAnnotations = AnnotationUtilityType.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);
                AnnotationUtility_SetGizmoEnabled = AnnotationUtilityType.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic);
                AnnotationUtility_SetIconEnabled = AnnotationUtilityType.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);

                if (AnnotationUtility_GetAnnotations == null)
                {
                    throw new MissingMemberException(AnnotationUtilityType.FullName, "GetAnnotations");
                }

                if (AnnotationUtility_SetGizmoEnabled == null)
                {
                    throw new MissingMemberException(AnnotationUtilityType.FullName, "SetGizmoEnabled");
                }

                if (AnnotationUtility_SetIconEnabled == null)
                {
                    throw new MissingMemberException(AnnotationUtilityType.FullName, "SetIconEnabled");
                }

                AnnotationType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.Annotation", true);
                Annotation_classID = AnnotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
                Annotation_scriptClass = AnnotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
                Annotation_iconEnabled = AnnotationType.GetField("iconEnabled", BindingFlags.Public | BindingFlags.Instance);
                Annotation_gizmoEnabled = AnnotationType.GetField("gizmoEnabled", BindingFlags.Public | BindingFlags.Instance);

                if (Annotation_classID == null)
                {
                    throw new MissingMemberException(AnnotationType.FullName, "classID");
                }

                if (Annotation_scriptClass == null)
                {
                    throw new MissingMemberException(AnnotationType.FullName, "scriptClass");
                }

                if (Annotation_iconEnabled == null)
                {
                    throw new MissingMemberException(AnnotationType.FullName, "iconEnabled");
                }

                if (Annotation_gizmoEnabled == null)
                {
                    throw new MissingMemberException(AnnotationType.FullName, "gizmoEnabled");
                }

                unityAnnotations = new Dictionary<int, Annotation>();
                scriptAnnotations = new Dictionary<string, Annotation>();

                var rawAnnotations = (Array)AnnotationUtility_GetAnnotations.Invoke(null, null);

                foreach (var rawAnnotation in rawAnnotations)
                {
                    var annotation = new Annotation(rawAnnotation);

                    if (annotation.classID == EditorUnityObjectUtility.MonoBehaviourClassID)
                    {
                        // Can happen when multiple types have the same name
                        if (!scriptAnnotations.ContainsKey(annotation.scriptClass))
                        {
                            scriptAnnotations.Add(annotation.scriptClass, annotation);
                        }
                    }
                    else
                    {
                        unityAnnotations.Add(annotation.classID, annotation);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
        }

        private static readonly Type AnnotationUtilityType;
        private static readonly MethodInfo AnnotationUtility_GetAnnotations;
        private static readonly MethodInfo AnnotationUtility_SetGizmoEnabled;
        private static readonly MethodInfo AnnotationUtility_SetIconEnabled;
        private static readonly Type AnnotationType;
        private static readonly FieldInfo Annotation_classID;
        private static readonly FieldInfo Annotation_scriptClass;
        private static readonly FieldInfo Annotation_iconEnabled;
        private static readonly FieldInfo Annotation_gizmoEnabled;

        private static readonly Dictionary<int, Annotation> unityAnnotations;
        private static readonly Dictionary<string, Annotation> scriptAnnotations;

        public static Annotation GetAnnotation(Type type)
        {
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                var scriptClass = EditorUnityObjectUtility.GetScriptClass(type);

                if (!scriptAnnotations.ContainsKey(scriptClass))
                {
                    return null;
                }

                return scriptAnnotations[scriptClass];
            }
            else
            {
                var classId = EditorUnityObjectUtility.GetClassID(type);

                if (!unityAnnotations.ContainsKey(classId))
                {
                    return null;
                }

                return unityAnnotations[classId];
            }
        }

        public static Annotation GetAnnotation<T>()
        {
            return GetAnnotation(typeof(T));
        }

        public class Annotation
        {
            public Annotation(object instance)
            {
                this.instance = instance;
                classID = (int)Annotation_classID.GetValue(instance);
                scriptClass = (string)Annotation_scriptClass.GetValue(instance);
            }

            private readonly object instance;

            public int classID { get; private set; }
            public string scriptClass { get; private set; }

            public bool iconEnabled
            {
                get
                {
                    try
                    {
                        return (bool)Annotation_iconEnabled.GetValue(instance);
                    }
                    catch (Exception ex)
                    {
                        throw new UnityEditorInternalException(ex);
                    }
                }
                set
                {
                    try
                    {
                        AnnotationUtility_SetIconEnabled.Invoke(null, new object[] { classID, scriptClass, value ? 1 : 0 });
                    }
                    catch (Exception ex)
                    {
                        throw new UnityEditorInternalException(ex);
                    }
                }
            }

            public bool gizmoEnabled
            {
                get
                {
                    try
                    {
                        return (bool)Annotation_gizmoEnabled.GetValue(instance);
                    }
                    catch (Exception ex)
                    {
                        throw new UnityEditorInternalException(ex);
                    }
                }
                set
                {
                    try
                    {
                        if (EditorApplicationUtility.unityVersion.major >= 2019)
                        {
                            AnnotationUtility_SetGizmoEnabled.Invoke(null, new object[] { classID, scriptClass, value ? 1 : 0, false });
                        }
                        else
                        {
                            AnnotationUtility_SetGizmoEnabled.Invoke(null, new object[] { classID, scriptClass, value ? 1 : 0 });
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new UnityEditorInternalException(ex);
                    }
                }
            }
        }
    }
}
