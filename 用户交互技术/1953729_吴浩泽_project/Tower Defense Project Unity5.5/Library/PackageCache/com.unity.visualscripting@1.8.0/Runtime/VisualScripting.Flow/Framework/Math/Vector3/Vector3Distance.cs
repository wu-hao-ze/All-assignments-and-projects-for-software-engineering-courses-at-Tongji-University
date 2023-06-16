namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the distance between two 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Distance")]
    public sealed class Vector3Distance : Distance<UnityEngine.Vector3>
    {
        public override float Operation(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
        {
            return UnityEngine.Vector3.Distance(a, b);
        }
    }
}
