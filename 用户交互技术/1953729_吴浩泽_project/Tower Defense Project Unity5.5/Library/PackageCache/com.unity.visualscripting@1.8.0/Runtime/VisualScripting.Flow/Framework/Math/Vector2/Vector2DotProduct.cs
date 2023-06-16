namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the dot product of two 2D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Dot Product")]
    public sealed class Vector2DotProduct : DotProduct<UnityEngine.Vector2>
    {
        public override float Operation(UnityEngine.Vector2 a, UnityEngine.Vector2 b)
        {
            return UnityEngine.Vector2.Dot(a, b);
        }
    }
}
