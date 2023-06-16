using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class UndoUtility
    {
        public static void RecordEditedObject(string name)
        {
            RecordObject(LudiqEditorUtility.editedObject, name);
        }

        private static void RecordObject(UnityObject uo, string name)
        {
            if (uo == null)
            {
                return;
            }

            // We can't use Undo.RecordObject, because Unity will attempt
            // to store only a diff of SerializedProperties on the undo stack
            // (likely in a way similar to how prefab modifications are stored).
            // This is memory efficient, however it may completely mess up
            // the order of lists/arrays that are meant to be treated holistically.
            // Instead, we have to use Undo.RegisterCompleteObjectUndo.

            // For example:
            // 1. Call RecordObject before deleting an item "x" from a serialized list
            // 2. Unity realizes this item was at index "i", and stores "remove x at i" on the undo stack
            // 3. Do any other operation that changes the overall order of your list
            // 4. Undo, and Unity will pull the opposite of 2, meaning "insert x at i".
            // However, the "i" index is no longer guaranteed to be that destined to "x".
            // If the list has to strictly be treated holistically (as a whole), we cannot
            // trust a list of differential operations to reconstitute it properly.

            // In our case, SerializationData.objectReferences is one such list.
            // It is always treated holistically by our FullSerializer object converter,
            // and therefore must always come as a whole, not a set of diffs.

            // Undo.RecordObject(uo, name); // BAD
            Undo.RegisterCompleteObjectUndo(uo, name); // GOOD

            // The documentation seems to imply this is automatically done
            // by calling Undo.RecordObject, however upon testing that doesn't
            // appear to be true. TODO: report bug to unity.
            // https://docs.unity3d.com/ScriptReference/EditorUtility.SetDirty.html
            if (!uo.IsSceneBound())
            {
                EditorUtility.SetDirty(uo);
            }

            // From the Unity documentation:
            /* Call this method after making modifications to an instance of a Prefab
             * to record those changes in the instance. If this method is not called,
             * changes made to the instance are lost. Note that if you are not using
             * SerializedProperty/SerializedObject, changes to the object are not recorded
             * in the undo system whether or not this method is called.
             */
            if (uo.IsPrefabInstance())
            {
                // One more catch: because we use RegisterCompleteObjectUndo
                // instead of RecordObject, the object isn't added to the internal list
                // of objects that need to be actually recorded when the undo
                // record is flushed.

                // The problem is that RecordPrefabInstancePropertyModifications must
                // be called *after* making modifications for it to work. This seems to
                // be done automatically from undo record flushing, but since our object
                // isn't registered for flushing, it will never get called.

                // There does not seem to be a way of registering an object for flushing
                // without incrementally recording its property changes.

                // The hacky workaround here is to wait one editor frame to actually
                // apply the modifications at a later time. Another solution would have
                // been to have manual BeginUndoCheck and EndUndoCheck methods,
                // but it would be more complicated on usage and well, this seems to work.

                EditorApplication.delayCall += () =>
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(uo);
                };
            }
        }
    }
}
