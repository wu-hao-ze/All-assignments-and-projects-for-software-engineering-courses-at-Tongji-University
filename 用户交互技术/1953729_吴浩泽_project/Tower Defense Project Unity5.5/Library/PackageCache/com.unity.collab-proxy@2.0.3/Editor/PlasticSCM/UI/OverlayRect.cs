using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class OverlayRect
    {
        internal static Rect GetOverlayRect(
            Rect selectionRect,
            float iconOffset)
        {
            if (selectionRect.width > selectionRect.height)
                return GetOverlayRectForSmallestSize(
                    selectionRect);

            return GetOverlayRectForOtherSizes(selectionRect, iconOffset);
        }

        internal static Rect GetCenteredRect(
                Rect selectionRect)
        {
            return new Rect(
                selectionRect.x + 3f,
                selectionRect.y + 1f,
                UnityConstants.OVERLAY_STATUS_ICON_SIZE,
                UnityConstants.OVERLAY_STATUS_ICON_SIZE);
        }

        static Rect GetOverlayRectForSmallestSize(
                    Rect selectionRect)
        {
            return new Rect(
                selectionRect.x + 5f,
                selectionRect.y + 4f,
                UnityConstants.OVERLAY_STATUS_ICON_SIZE,
                UnityConstants.OVERLAY_STATUS_ICON_SIZE);
        }

        static Rect GetOverlayRectForOtherSizes(
            Rect selectionRect,
            float iconOffset)
        {
            float widthRatio = selectionRect.width / 
                UNITY_STANDARD_ICON_SIZE;
            float heightRatio = selectionRect.height / 
                UNITY_STANDARD_ICON_SIZE;

            return new Rect(
               selectionRect.x + (iconOffset * widthRatio) - 1f,
               selectionRect.y + (iconOffset * heightRatio) - 13f,
               UnityConstants.OVERLAY_STATUS_ICON_SIZE * widthRatio,
               UnityConstants.OVERLAY_STATUS_ICON_SIZE * heightRatio);
        }

        const int UNITY_STANDARD_ICON_SIZE = 32;
    }
}