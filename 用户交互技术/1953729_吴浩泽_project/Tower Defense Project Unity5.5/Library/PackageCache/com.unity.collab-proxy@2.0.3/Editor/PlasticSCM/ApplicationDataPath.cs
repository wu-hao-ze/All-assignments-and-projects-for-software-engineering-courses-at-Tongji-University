using UnityEngine;

namespace Unity.PlasticSCM.Editor
{
    internal static class ApplicationDataPath
    {
        internal static string Get()
        {
            return mApplicationDataPath ?? Application.dataPath;
        }

        internal static void InitializeForTesting(string applicationDataPath)
        {
            mApplicationDataPath = applicationDataPath;
        }

        internal static void Reset()
        {
            mApplicationDataPath = null;
        }

        static string mApplicationDataPath;
    }
}
