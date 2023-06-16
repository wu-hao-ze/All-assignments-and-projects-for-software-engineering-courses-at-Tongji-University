using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DrawTextBlockWithEndLink
    {
        internal static void For(
            string url,
            string formattedExplanation,
            GUIStyle textblockStyle)
        {
            string explanation = string.Format(
                formattedExplanation, "");

            GUILayout.Label(explanation, textblockStyle);

            if (explanation == formattedExplanation)
                return;

            string coloredUrl = string.Format(
                "<color=\"{0}\">{1}</color>",
                UnityStyles.HexColors.LINK_COLOR,
                url);

            float linkWidth =
                textblockStyle.CalcSize(new GUIContent(url)).x;

            if (GUILayout.Button(coloredUrl, textblockStyle, GUILayout.Width(linkWidth)))
                Application.OpenURL(url);

            EditorGUIUtility.AddCursorRect(
                GUILayoutUtility.GetLastRect(), MouseCursor.Link);
        }
    }
}
