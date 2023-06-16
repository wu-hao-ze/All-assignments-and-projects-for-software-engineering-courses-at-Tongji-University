using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class UnityObjectOwnershipUtility
    {
        public static void CopyOwner(object source, object destination)
        {
            var destinationOwned = destination as IUnityObjectOwnable;

            if (destinationOwned != null)
            {
                destinationOwned.owner = GetOwner(source);
            }
        }

        public static void RemoveOwner(object o)
        {
            var sourceOwned = o as IUnityObjectOwnable;

            if (sourceOwned != null)
            {
                sourceOwned.owner = null;
            }
        }

        public static UnityObject GetOwner(object o)
        {
            return (o as Component)?.gameObject ?? (o as IUnityObjectOwnable)?.owner;
        }
    }
}
