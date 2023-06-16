using System;
using UnityEngine;

namespace UnityEditor.Timeline
{
    readonly struct OverlayDrawer
    {
        enum OverlayType
        {
            BackgroundColor,
            BackgroundTexture,
            TextBox
        }

        readonly OverlayType m_Type;
        readonly Rect m_Rect;
        readonly string m_Text;
        readonly Texture2D m_Texture;
        readonly Color m_Color;
        readonly GUIStyle m_BackgroundTextStyle;
        readonly GUIStyle m_TextStyle;

        OverlayDrawer(Rect rectangle, Color backgroundColor)
        {
            m_Type = OverlayType.BackgroundColor;
            m_Rect = rectangle;
            m_Color = backgroundColor;
            m_Text = string.Empty;
            m_Texture = null;
            m_BackgroundTextStyle = null;
            m_TextStyle = null;
        }

        OverlayDrawer(Rect rectangle, Texture2D backTexture)
        {
            m_Type = OverlayType.BackgroundTexture;
            m_Rect = rectangle;
            m_Color = Color.clear;
            m_Text = string.Empty;
            m_Texture = backTexture;
            m_BackgroundTextStyle = null;
            m_TextStyle = null;
        }

        OverlayDrawer(Rect rectangle, string msg, GUIStyle textStyle, Color textColor, Color bgTextColor, GUIStyle bgTextStyle)
        {
            m_Type = OverlayType.TextBox;
            m_Rect = rectangle;
            m_Text = msg;
            m_TextStyle = textStyle;
            m_TextStyle.normal.textColor = textColor;
            m_BackgroundTextStyle = bgTextStyle;
            m_BackgroundTextStyle.normal.textColor = bgTextColor;
            m_Texture = null;
            m_Color = Color.clear;
        }

        public static OverlayDrawer CreateColorOverlay(Rect rectangle, Color backgroundColor)
        {
            return new OverlayDrawer(rectangle, backgroundColor);
        }

        public static OverlayDrawer CreateTextureOverlay(Rect rectangle, Texture2D backTexture)
        {
            return new OverlayDrawer(rectangle, backTexture);
        }

        public static OverlayDrawer CreateTextBoxOverlay(Rect rectangle, string msg, GUIStyle textStyle, Color textColor, Color bgTextColor, GUIStyle bgTextStyle)
        {
            return new OverlayDrawer(rectangle, msg, textStyle, textColor, bgTextColor, bgTextStyle);
        }

        public void Draw()
        {
            Rect overlayRect = GUIClip.Clip(m_Rect);
            switch (m_Type)
            {
                case OverlayType.BackgroundColor:
                    EditorGUI.DrawRect(overlayRect, m_Color);
                    break;
                case OverlayType.BackgroundTexture:
                    Graphics.DrawTextureRepeated(overlayRect, m_Texture);
                    break;
                case OverlayType.TextBox:
                {
                    using (new GUIColorOverride(m_BackgroundTextStyle.normal.textColor))
                        GUI.Box(overlayRect, GUIContent.none, m_BackgroundTextStyle);
                    Graphics.ShadowLabel(overlayRect, GUIContent.Temp(m_Text), m_TextStyle, m_TextStyle.normal.textColor, Color.black);
                    break;
                }
            }
        }
    }
}
