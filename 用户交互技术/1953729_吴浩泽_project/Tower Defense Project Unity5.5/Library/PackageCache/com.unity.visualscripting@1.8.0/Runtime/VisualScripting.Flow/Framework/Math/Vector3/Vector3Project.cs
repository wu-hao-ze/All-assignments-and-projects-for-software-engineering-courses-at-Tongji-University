namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the projection of a 3D vector on another.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Project")]
    public sealed class Vector3Project : Project<UnityEngine.Vector3>
    {
        public override UnityEngine.Vector3 Operation(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
        {
            return UnityEngine.Vector3.Project(a, b);
        }
    }
}
