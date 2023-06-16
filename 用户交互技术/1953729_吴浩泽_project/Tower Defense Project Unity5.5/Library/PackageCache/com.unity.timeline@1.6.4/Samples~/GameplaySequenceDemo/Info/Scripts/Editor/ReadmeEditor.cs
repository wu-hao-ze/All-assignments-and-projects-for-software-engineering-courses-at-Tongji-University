using UnityEngine;
using UnityEditor;
using System;

namespace Samples.GameplaySequence
{
    [CustomEditor(typeof(Readme))]
    public class ReadmeEditor : Editor
    {
        const float k_Space = 16f;

        protected override void OnHeaderGUI()
        {
            var readme = (Readme)target;
            Init();

            float iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);

            GUILayout.BeginHorizontal("In BigTitle");
            {
                GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
                GUILayout.Label(readme.title, titleStyle);
            }
            GUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            var readme = (Readme)target;
            Init();

            foreach (Readme.Section section in readme.sections)
            {
                if (!string.IsNullOrEmpty(section.heading))
                {
                    GUILayout.Label(section.heading, headingStyle);
                }

                if (!string.IsNullOrEmpty(section.text))
                {
                    GUILayout.Label(section.text, bodyStyle);
                }

                if (!string.IsNullOrEmpty(section.linkText))
                {
                    if (LinkLabel(new GUIContent(section.linkText)))
                    {
                        Application.OpenURL(section.url);
                    }
                }

                GUILayout.Space(k_Space);
            }
        }

        bool m_Initialized;

        [SerializeField] GUIStyle m_TitleStyle;
        [SerializeField] GUIStyle m_HeadingStyle;
        [SerializeField] GUIStyle m_LinkStyle;
        [SerializeField] GUIStyle m_BodyStyle;

        GUIStyle linkStyle => m_LinkStyle;
        GUIStyle titleStyle => m_TitleStyle;
        GUIStyle headingStyle => m_HeadingStyle;
        GUIStyle bodyStyle => m_BodyStyle;

        void Init()
        {
            if (m_Initialized)
                return;

            m_BodyStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                fontSize = 14
            };

            m_TitleStyle = new GUIStyle(m_BodyStyle)
            {
                fontSize = 26
            };

            m_HeadingStyle = new GUIStyle(m_BodyStyle)
            {
                fontSize = 18
            };

            // Match selection color which works nicely for both light and dark skins
            m_LinkStyle = new GUIStyle(m_BodyStyle)
            {
                wordWrap = false,
                normal = { textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f) },
                stretchWidth = false
            };

            m_Initialized = true;
        }

        bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = GUILayoutUtility.GetRect(label, linkStyle, options);

            Handles.BeginGUI();
            Handles.color = linkStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

            return GUI.Button(position, label, linkStyle);
        }
    }
}
