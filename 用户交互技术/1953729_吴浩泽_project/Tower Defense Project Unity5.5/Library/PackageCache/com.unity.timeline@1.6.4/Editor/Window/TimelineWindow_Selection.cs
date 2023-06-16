using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    partial class TimelineWindow
    {
        [SerializeField]
        SequencePath m_SequencePath;

        void OnSelectionChange()
        {
            //Sanitize the inline curve selection
            SelectionManager.GetCurrentInlineEditorCurve()?.ValidateCurvesSelection();

            RefreshSelection(false);
        }

        void RefreshSelection(bool forceRebuild)
        {
            // if we're in Locked mode, keep current selection - don't use locked property because the
            // sequence hierarchy may need to be rebuilt and it assumes no asset == unlocked
            if (m_LockTracker.isLocked || (state != null && state.recording))
            {
                RestoreLastSelection(forceRebuild);
                return;
            }

            // selection is a TimelineAsset
            Object selectedObject = Selection.activeObject as TimelineAsset;
            if (selectedObject != null)
            {
                SetCurrentSelection(Selection.activeObject);
                return;
            }

            // selection is a GameObject, or a prefab with a director
            var selectedGO = Selection.activeGameObject;
            if (selectedGO != null)
            {
                bool isSceneObject = !PrefabUtility.IsPartOfPrefabAsset(selectedGO);
                bool hasDirector = selectedGO.GetComponent<PlayableDirector>() != null;
                if (isSceneObject || hasDirector)
                {
                    SetCurrentSelection(selectedGO);
                    return;
                }
            }

            //If not currently editing a Timeline and the selection is empty, clear selection
            if (Selection.activeObject == null &&
                state.IsEditingAnEmptyTimeline())
            {
                SetCurrentSelection(null);
            }


            // otherwise, keep the same selection.
            RestoreLastSelection(forceRebuild);
        }

        void RestoreLastSelection(bool forceRebuild)
        {
            state.SetCurrentSequencePath(m_SequencePath, forceRebuild);

            //case 1201405 and 1278598: unlock the window if there is no valid asset, since the lock button is disabled
            if (m_LockTracker.isLocked && state.editSequence.asset == null)
                m_LockTracker.isLocked = false;
        }

        void SetCurrentSelection(Object obj)
        {
            var selectedGameObject = obj as GameObject;
            if (selectedGameObject != null)
            {
                PlayableDirector director = TimelineUtility.GetDirectorComponentForGameObject(selectedGameObject);
                SetTimeline(director);
            }
            else
            {
                var selectedSequenceAsset = obj as TimelineAsset;
                if (selectedSequenceAsset != null)
                {
                    SetTimeline(selectedSequenceAsset);
                }
            }

            Repaint();
        }
    }
}
