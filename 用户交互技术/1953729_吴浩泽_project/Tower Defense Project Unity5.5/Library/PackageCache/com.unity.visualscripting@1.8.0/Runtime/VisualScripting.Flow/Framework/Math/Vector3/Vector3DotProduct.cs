namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the dot product of two 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Dot Product")]
    public sealed class Vector3DotProduct : DotProduct<UnityEngine.Vector3>
    {
        public override float Operation(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
        {
            return UnityEngine.Vector3.Dot(a, b);
        }
    }
}
