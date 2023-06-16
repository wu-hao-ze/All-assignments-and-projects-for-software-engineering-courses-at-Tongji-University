using System;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace Timeline.Samples
{
    // Editor used by the Timeline window to customize the appearance of an AnnotationMarker
    [CustomTimelineEditor(typeof(AnnotationMarker))]
    public class AnnotationMarkerEditor : MarkerEditor
    {
        const float k_LineOverlayWidth = 6.0f;

        const string k_OverlayPath = "timeline_annotation_overlay";
        const string k_OverlaySelectedPath = "timeline_annotation_overlay_selected";
        const string k_OverlayCollapsedPath = "timeline_annotation_overlay_collapsed";

        static Texture2D s_OverlayTexture;
        static Texture2D s_OverlaySelectedTexture;
        static Texture2D s_OverlayCollapsedTexture;

        static AnnotationMarkerEditor()
        {
            s_OverlayTexture = Resources.Load<Texture2D>(k_OverlayPath);
            s_OverlaySelectedTexture = Resources.Load<Texture2D>(k_OverlaySelectedPath);
            s_OverlayCollapsedTexture = Resources.Load<Texture2D>(k_OverlayCollapsedPath);
        }

        // Draws a vertical line on top of the Timeline window's contents.
        public override void DrawOverlay(IMarker marker, MarkerUIStates uiState, MarkerOverlayRegion region)
        {
            // The `marker argument needs to be cast as the appropriate type, usually the one specified in the `CustomTimelineEditor` attribute
            AnnotationMarker annotation = marker as AnnotationMarker;
            if (annotation == null)
            {
                return;
            }

            if (annotation.showLineOverlay)
            {
                DrawLineOverlay(annotation.color, region);
            }

            DrawColorOverlay(region, annotation.color, uiState);
        }

        // Sets the marker's tooltip based on its title.
        public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
        {
            // The `marker argument needs to be cast as the appropriate type, usually the one specified in the `CustomTimelineEditor` attribute
            AnnotationMarker annotation = marker as AnnotationMarker;
            if (annotation == null)
            {
                return base.GetMarkerOptions(marker);
            }

            return new MarkerDrawOptions { tooltip = annotation.title };
        }

        static void DrawLineOverlay(Color color, MarkerOverlayRegion region)
        {
            // Calculate markerRegion's center on the x axis
            float markerRegionCenterX = region.markerRegion.xMin + (region.markerRegion.width - k_LineOverlayWidth) / 2.0f;

            // Calculate a rectangle that uses the full timeline region's height
            Rect overlayLineRect = new Rect(markerRegionCenterX,
                region.timelineRegion.y,
                k_LineOverlayWidth,
                region.timelineRegion.height);

            Color overlayLineColor = new Color(color.r, color.g, color.b, color.a * 0.5f);
            EditorGUI.DrawRect(overlayLineRect, overlayLineColor);
        }

        static void DrawColorOverlay(MarkerOverlayRegion region, Color color, MarkerUIStates state)
        {
            // Save the Editor's overlay color before changing it
            Color oldColor = GUI.color;
            GUI.color = color;

            if (state.HasFlag(MarkerUIStates.Selected))
            {
                GUI.DrawTexture(region.markerRegion, s_OverlaySelectedTexture);
            }
            else if (state.HasFlag(MarkerUIStates.Collapsed))
            {
                GUI.DrawTexture(region.markerRegion, s_OverlayCollapsedTexture);
            }
            else if (state.HasFlag(MarkerUIStates.None))
            {
                GUI.DrawTexture(region.markerRegion, s_OverlayTexture);
            }

            // Restore the previous Editor's overlay color
            GUI.color = oldColor;
        }
    }
}
