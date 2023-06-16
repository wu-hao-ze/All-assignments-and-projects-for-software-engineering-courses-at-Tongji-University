using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Unity.VisualScripting
{
    public static class PackageVersionUtility
    {
        private static SemanticVersion _version;

        public static SemanticVersion version
        {
            get
            {
                if (_version.IsUnset())
                {
                    var myPackage = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.visualscripting");
                    if (myPackage == null)
                        throw new Exception("Error! Visual Scripting: couldn't find visual scripting package");

                    var couldParse = SemanticVersion.TryParse(myPackage.version, out var parsedVersion);
                    if (!couldParse)
                        throw new Exception("Error! Visual Scripting: couldn't parse package version");

                    _version = parsedVersion;
                }

                return _version;
            }
        }
    }
}
