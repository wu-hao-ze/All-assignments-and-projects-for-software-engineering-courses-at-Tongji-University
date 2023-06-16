namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the distance between two 2D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Distance")]
    public sealed class Vector2Distance : Distance<UnityEngine.Vector2>
    {
        public override float Operation(UnityEngine.Vector2 a, UnityEngine.Vector2 b)
        {
            return UnityEngine.Vector2.Distance(a, b);
        }
    }
}
