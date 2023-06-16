using System;

using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class MeasureMaxWidth
    {
        internal static float ForTexts(GUIStyle style, params string[] texts)
        {
            float result = 0;

            GUIContent content = new GUIContent();

            foreach (string text in texts)
            {
                content.text = text;

                result = Math.Max(result,
                    style.CalcSize(content).x);
            }

            return result + 10;
        }
    }
}
