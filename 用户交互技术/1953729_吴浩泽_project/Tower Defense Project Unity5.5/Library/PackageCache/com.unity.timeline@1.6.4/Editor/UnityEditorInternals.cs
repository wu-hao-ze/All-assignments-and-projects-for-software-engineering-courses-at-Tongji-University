using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.Timeline
{
    static class UnityEditorInternals
    {
        static readonly EditorGUI.ObjectFieldValidator k_AllowAllObjectsValidator = (references, type, property, options) => references.Length > 0 ? references[0] : null;

        public static Object DoObjectField(Rect position, Object obj, Type type, int controlId, bool allowScene, bool allowAllObjects = false)
        {
            EditorGUI.ObjectFieldValidator validator = null;
            if (allowAllObjects)
                validator = k_AllowAllObjectsValidator;

#if UNITY_2020_1_OR_NEWER
            var newObject = EditorGUI.DoObjectField(position, position, controlId, obj, null, type, validator, allowScene, EditorStyles.objectField);
#else
            var newObject = EditorGUI.DoObjectField(position, position, controlId, obj, type, null, validator, allowScene, EditorStyles.objectField);
#endif
            return newObject;
        }
    }
}
