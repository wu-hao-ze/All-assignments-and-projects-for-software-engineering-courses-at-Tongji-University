using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IFuzzyOption
    {
        object value { get; }
        bool parentOnly { get; }

        string label { get; }
        EditorTexture icon { get; }
        GUIStyle style { get; }

        string headerLabel { get; }
        bool showHeaderIcon { get; }

        bool hasFooter { get; }
        float GetFooterHeight(float width);
        void OnFooterGUI(Rect position);

        void OnPopulate();
    }
}
