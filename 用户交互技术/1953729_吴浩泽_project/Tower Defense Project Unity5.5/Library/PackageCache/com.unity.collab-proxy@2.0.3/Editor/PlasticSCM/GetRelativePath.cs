using System;

using UnityEngine;

namespace Unity.PlasticSCM.Editor
{
    internal static class GetRelativePath
    {
        internal static string ToApplication(string path)
        {
            Uri relativeToUri = new Uri(ApplicationDataPath.Get());
            Uri pathUri = new Uri(FixVolumeLetterPath(path));

            return Uri.UnescapeDataString(
                relativeToUri.MakeRelativeUri(pathUri).ToString());
        }

        static string FixVolumeLetterPath(string path)
        {
            string volumeLetter = new string(new char[] { path[0] });
            volumeLetter = volumeLetter.ToUpperInvariant();

            return string.Concat(volumeLetter, path.Substring(1));
        }
    }
}
