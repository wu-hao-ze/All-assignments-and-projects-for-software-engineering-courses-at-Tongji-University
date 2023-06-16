namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the node length version of a 2D vector.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Normalize")]
    public sealed class Vector2Normalize : Normalize<UnityEngine.Vector2>
    {
        public override UnityEngine.Vector2 Operation(UnityEngine.Vector2 input)
        {
            return input.normalized;
        }
    }
}
