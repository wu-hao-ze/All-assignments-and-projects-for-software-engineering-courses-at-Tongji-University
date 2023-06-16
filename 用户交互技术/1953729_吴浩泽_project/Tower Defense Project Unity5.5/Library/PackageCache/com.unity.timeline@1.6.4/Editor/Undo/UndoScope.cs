using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// Disposable scope object used to collect multiple items for Undo.
    ///     Automatically filters out duplicates
    /// </summary>
    struct UndoScope : IDisposable
    {
        private static readonly HashSet<UnityEngine.Object> s_ObjectsToUndo = new HashSet<Object>();
        private string m_Name;

        public UndoScope(string name)
        {
            m_Name = name;
        }

        public void Dispose()
        {
            ApplyUndo(m_Name);
        }

        public void AddObject(UnityEngine.Object asset)
        {
            if (asset != null)
                s_ObjectsToUndo.Add(asset);
        }

        public void AddClip(TimelineClip clip, bool includeAsset)
        {
            if (clip != null && clip.GetParentTrack() != null)
                s_ObjectsToUndo.Add(clip.GetParentTrack());
            if (includeAsset && clip != null && clip.asset != null)
                s_ObjectsToUndo.Add(clip.asset);
        }

        public void Add(IEnumerable<TrackAsset> tracks)
        {
            if (tracks == null)
                return;

            foreach (var track in tracks)
                AddObject(track);
        }

        public void Add(IEnumerable<TimelineClip> clips, bool includeAssets)
        {
            if (clips == null)
                return;

            foreach (var clip in clips)
            {
                AddClip(clip, includeAssets);
            }
        }

        public void Add(IEnumerable<IMarker> markers)
        {
            if (markers == null)
                return;

            foreach (var marker in markers)
            {
                if (marker is Object o)
                    AddObject(o);
                else if (marker != null)
                    AddObject(marker.parent);
            }
        }

        private static void ApplyUndo(string name)
        {
            if (s_ObjectsToUndo.Count == 1)
                TimelineUndo.PushUndo(s_ObjectsToUndo.First(), name);
            else if (s_ObjectsToUndo.Count > 1)
                TimelineUndo.PushUndo(s_ObjectsToUndo.ToArray(), name);
            s_ObjectsToUndo.Clear();
        }
    }
}
