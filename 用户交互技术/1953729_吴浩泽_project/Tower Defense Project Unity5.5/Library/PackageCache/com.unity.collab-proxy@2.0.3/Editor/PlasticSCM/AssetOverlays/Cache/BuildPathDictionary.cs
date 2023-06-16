using System;
using System.Collections.Generic;

using Codice.Utils;

namespace Unity.PlasticSCM.Editor.AssetsOverlays.Cache
{
    internal static class BuildPathDictionary
    {
        internal static Dictionary<string, T> ForPlatform<T>()
        {
            if (PlatformIdentifier.IsWindows())
                return new Dictionary<string, T>(
                    StringComparer.OrdinalIgnoreCase);

            return new Dictionary<string, T>();
        }
    }
}
