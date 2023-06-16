namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the node length version of a 4D vector.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Normalize")]
    public sealed class Vector4Normalize : Normalize<UnityEngine.Vector4>
    {
        public override UnityEngine.Vector4 Operation(UnityEngine.Vector4 input)
        {
            return UnityEngine.Vector4.Normalize(input);
        }
    }
}
