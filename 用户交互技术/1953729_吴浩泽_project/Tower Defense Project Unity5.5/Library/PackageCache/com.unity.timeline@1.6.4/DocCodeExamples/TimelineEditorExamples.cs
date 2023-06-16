using UnityEditor.Timeline;

namespace DocCodeExamples
{
    class TimelineEditorExamples_HideAPI
    {
        void RefreshReasonExample()
        {
            #region declare-refreshReason

            TimelineEditor.Refresh(RefreshReason.ContentsModified | RefreshReason.SceneNeedsUpdate);

            #endregion
        }
    }
}
