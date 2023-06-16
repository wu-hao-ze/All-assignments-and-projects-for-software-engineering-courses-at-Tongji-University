namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the dot product of two 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Dot Product")]
    public sealed class Vector4DotProduct : DotProduct<UnityEngine.Vector4>
    {
        public override float Operation(UnityEngine.Vector4 a, UnityEngine.Vector4 b)
        {
            return UnityEngine.Vector4.Dot(a, b);
        }
    }
}
