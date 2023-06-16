using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Timeline
{
    static class TimelineUndo
    {
        public static void PushDestroyUndo(TimelineAsset timeline, Object thingToDirty, Object objectToDestroy)
        {
#if UNITY_EDITOR
            if (objectToDestroy == null || !DisableUndoGuard.enableUndo)
                return;

            if (thingToDirty != null)
                EditorUtility.SetDirty(thingToDirty);

            if (timeline != null)
                EditorUtility.SetDirty(timeline);

            Undo.DestroyObjectImmediate(objectToDestroy);
#else
            if (objectToDestroy != null)
                Object.Destroy(objectToDestroy);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void PushUndo(Object[] thingsToDirty, string operation)
        {
#if UNITY_EDITOR
            if (thingsToDirty == null || !DisableUndoGuard.enableUndo)
                return;

            for (var i = 0; i < thingsToDirty.Length; i++)
            {
                if (thingsToDirty[i] is TrackAsset track)
                    track.MarkDirty();
                EditorUtility.SetDirty(thingsToDirty[i]);
            }
            Undo.RegisterCompleteObjectUndo(thingsToDirty, UndoName(operation));
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void PushUndo(Object thingToDirty, string operation)
        {
#if UNITY_EDITOR
            if (thingToDirty != null && DisableUndoGuard.enableUndo)
            {
                var track = thingToDirty as TrackAsset;
                if (track != null)
                    track.MarkDirty();

                EditorUtility.SetDirty(thingToDirty);
                Undo.RegisterCompleteObjectUndo(thingToDirty, UndoName(operation));
            }
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RegisterCreatedObjectUndo(Object thingCreated, string operation)
        {
#if UNITY_EDITOR
            if (DisableUndoGuard.enableUndo)
            {
                Undo.RegisterCreatedObjectUndo(thingCreated, UndoName(operation));
            }
#endif
        }

        private static string UndoName(string name) => "Timeline " + name;

#if UNITY_EDITOR
        internal struct DisableUndoGuard : IDisposable
        {
            internal static bool enableUndo = true;
            static readonly Stack<bool> m_UndoStateStack = new Stack<bool>();
            bool m_Disposed;
            public DisableUndoGuard(bool disable)
            {
                m_Disposed = false;
                m_UndoStateStack.Push(enableUndo);
                enableUndo = !disable;
            }

            public void Dispose()
            {
                if (!m_Disposed)
                {
                    if (m_UndoStateStack.Count == 0)
                    {
                        Debug.LogError("UnMatched DisableUndoGuard calls");
                        enableUndo = true;
                        return;
                    }
                    enableUndo = m_UndoStateStack.Pop();
                    m_Disposed = true;
                }
            }
        }
#endif
    }
}
