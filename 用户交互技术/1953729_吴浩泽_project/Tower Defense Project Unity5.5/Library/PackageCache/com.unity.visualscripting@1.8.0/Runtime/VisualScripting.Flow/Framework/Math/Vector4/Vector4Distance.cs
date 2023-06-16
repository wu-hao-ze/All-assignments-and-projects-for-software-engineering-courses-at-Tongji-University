namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the distance between two 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Distance")]
    public sealed class Vector4Distance : Distance<UnityEngine.Vector4>
    {
        public override float Operation(UnityEngine.Vector4 a, UnityEngine.Vector4 b)
        {
            return UnityEngine.Vector4.Distance(a, b);
        }
    }
}
