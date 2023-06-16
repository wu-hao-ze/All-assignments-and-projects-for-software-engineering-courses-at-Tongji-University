namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the angle between two 2D vectors in degrees.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Angle")]
    public sealed class Vector2Angle : Angle<UnityEngine.Vector2>
    {
        public override float Operation(UnityEngine.Vector2 a, UnityEngine.Vector2 b)
        {
            return UnityEngine.Vector2.Angle(a, b);
        }
    }
}
