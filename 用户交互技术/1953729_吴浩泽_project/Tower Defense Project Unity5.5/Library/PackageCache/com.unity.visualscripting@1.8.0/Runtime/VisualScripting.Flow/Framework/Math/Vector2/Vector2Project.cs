namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the projection of a 2D vector on another.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Project")]
    public sealed class Vector2Project : Project<UnityEngine.Vector2>
    {
        public override UnityEngine.Vector2 Operation(UnityEngine.Vector2 a, UnityEngine.Vector2 b)
        {
            return UnityEngine.Vector2.Dot(a, b) * b.normalized;
        }
    }
}
