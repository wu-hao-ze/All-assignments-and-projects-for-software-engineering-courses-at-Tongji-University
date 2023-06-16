using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public interface IUnityObjectOwnable
    {
        UnityObject owner { get; set; }
    }
}
