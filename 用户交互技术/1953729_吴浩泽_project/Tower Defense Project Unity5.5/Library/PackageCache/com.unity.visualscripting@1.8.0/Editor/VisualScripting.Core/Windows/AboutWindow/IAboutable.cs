using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IAboutable
    {
        string name { get; }
        string description { get; }
        Texture2D logo { get; }
        string author { get; }
        string authorLabel { get; }
        Texture2D authorLogo { get; }
        string copyrightHolder { get; }
        int copyrightYear { get; }
        string url { get; }
        string authorUrl { get; }
        SemanticVersion version { get; }
    }
}
