using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class AssetBundleUtility
    {
        public static bool IsLoaded(this AssetBundle bundle)
        {
            return AssetBundle.GetAllLoadedAssetBundles().Any(b => b == bundle);
        }
    }
}
