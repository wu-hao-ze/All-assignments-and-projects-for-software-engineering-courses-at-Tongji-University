using System.Collections.Generic;
using UnityEditor;

namespace Unity.VisualScripting
{
    public abstract class OptimizedEditor<TIndividual> : UnityEditor.Editor
        where TIndividual : IndividualEditor, new()
    {
        private TIndividual GetIndividualDrawer(SerializedObject serializedObject)
        {
            var hash = serializedObject.GetHashCode();

            if (!individualDrawers.ContainsKey(hash))
            {
                var individualDrawer = new TIndividual();
                individualDrawer.Initialize(serializedObject, this);
                individualDrawers.Add(hash, individualDrawer);
            }

            return individualDrawers[hash];
        }

        public sealed override void OnInspectorGUI()
        {
            if (serializedObject.isEditingMultipleObjects)
            {
                EditorGUILayout.HelpBox("Multi-object editing is not supported.", MessageType.Info);
                return;
            }

            GetIndividualDrawer(serializedObject).OnGUI();
        }

        static OptimizedEditor()
        {
            individualDrawers = new Dictionary<int, TIndividual>();
            EditorApplicationUtility.onSelectionChange += ClearCache;
        }

        private static Dictionary<int, TIndividual> individualDrawers;

        private static void ClearCache()
        {
            try
            {
                foreach (var individualDrawer in individualDrawers.Values)
                {
                    individualDrawer.Dispose();
                }
            }
            finally
            {
                individualDrawers.Clear();
            }
        }
    }
}
