using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public interface IResourceProvider
    {
        IEnumerable<string> GetAllFiles();

        IEnumerable<string> GetFiles(string path);

        IEnumerable<string> GetDirectories(string path);

        string GetPersonalPath(string path, float width);
        string GetProfessionalPath(string path, float width);

        bool FileExists(string path);

        bool DirectoryExists(string path);

        string DebugPath(string path);

        T LoadAsset<T>(string path) where T : UnityObject;

        Texture2D LoadTexture(string path, CreateTextureOptions options);
    }
}
