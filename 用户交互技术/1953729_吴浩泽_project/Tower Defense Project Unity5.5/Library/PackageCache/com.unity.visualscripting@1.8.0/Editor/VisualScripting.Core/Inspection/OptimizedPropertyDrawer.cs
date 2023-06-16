using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class OptimizedPropertyDrawer<TIndividual> : PropertyDrawer
        where TIndividual : IndividualPropertyDrawer, new()
    {
        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
            {
                return LudiqGUIUtility.HelpBoxHeight;
            }

            return GetIndividualDrawer(property).GetHeight(label);
        }

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUI.HelpBox(position, "Multiple object editing is not supported.", MessageType.Info);
                return;
            }

            GetIndividualDrawer(property).OnGUI(position, label);
        }

        static OptimizedPropertyDrawer()
        {
            individualDrawers = new Dictionary<int, TIndividual>();
            EditorApplicationUtility.onSelectionChange += ClearCache;
        }

        private static Dictionary<int, TIndividual> individualDrawers;

        private static void ClearCache()
        {
            foreach (var individualDrawer in individualDrawers.Values)
            {
                individualDrawer.Dispose();
            }

            individualDrawers.Clear();
        }

        private static TIndividual GetIndividualDrawer(SerializedProperty property)
        {
            var hash = SerializedPropertyUtility.GetPropertyHash(property);

            if (!individualDrawers.ContainsKey(hash))
            {
                var individualDrawer = new TIndividual();
                individualDrawer.Initialize(property);
                individualDrawers.Add(hash, individualDrawer);
                // Debug.LogFormat("Creating drawer for '{0}.{1}'.\n", property.serializedObject.targetObject.name, property.propertyPath);
            }

            return individualDrawers[hash];
        }
    }
}
