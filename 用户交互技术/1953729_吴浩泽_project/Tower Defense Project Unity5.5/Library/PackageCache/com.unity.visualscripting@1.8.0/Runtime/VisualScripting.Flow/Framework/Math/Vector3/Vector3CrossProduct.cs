namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the cross product of two 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Cross Product")]
    public sealed class Vector3CrossProduct : CrossProduct<UnityEngine.Vector3>
    {
        public override UnityEngine.Vector3 Operation(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
        {
            return UnityEngine.Vector3.Cross(a, b);
        }
    }
}
