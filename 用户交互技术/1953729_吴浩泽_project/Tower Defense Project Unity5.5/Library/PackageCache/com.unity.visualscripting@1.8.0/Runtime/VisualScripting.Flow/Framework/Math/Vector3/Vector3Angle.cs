namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the angle between two 3D vectors in degrees.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Angle")]
    public sealed class Vector3Angle : Angle<UnityEngine.Vector3>
    {
        public override float Operation(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
        {
            return UnityEngine.Vector3.Angle(a, b);
        }
    }
}
