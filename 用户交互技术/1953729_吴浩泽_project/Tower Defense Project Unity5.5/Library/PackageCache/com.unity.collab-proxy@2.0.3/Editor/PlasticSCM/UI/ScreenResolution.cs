using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class ScreenResolution
    {
        internal static string Get()
        {
            return string.Format("{0}x{1}",
                Screen.currentResolution.width,
                Screen.currentResolution.height);
        }
    }
}