using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IDescription
    {
        string title { get; }
        string summary { get; }
        EditorTexture icon { get; }
    }

    public static class XDescription
    {
        public static GUIContent ToGUIContent(this IDescription description)
        {
            return new GUIContent(description.title, null, description.summary);
        }

        public static GUIContent ToGUIContent(this IDescription description, int iconSize)
        {
            return new GUIContent(description.title, description.icon?[iconSize], description.summary);
        }
    }
}
