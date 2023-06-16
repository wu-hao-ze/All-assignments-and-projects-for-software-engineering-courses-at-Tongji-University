using UnityEngine;

namespace Unity.VisualScripting
{
    public interface ISidebarPanelContent
    {
        object sidebarControlHint { get; }

        GUIContent titleContent { get; }

        Vector2 minSize { get; }

        float GetHeight(float width);

        void OnGUI(Rect position);
    }
}
