using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

namespace DocCodeExamples
{
    class MarkerEditorExamples
    {
        void MarkerRegionExample(MarkerOverlayRegion region)
        {
            #region declare-trackRegion

            GUI.BeginClip(region.trackRegion, -region.trackRegion.min, Vector2.zero, false);
            EditorGUI.DrawRect(region.markerRegion, Color.blue);
            GUI.EndClip();

            #endregion
        }
    }
}
