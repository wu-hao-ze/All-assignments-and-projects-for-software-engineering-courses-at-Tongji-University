namespace Unity.VisualScripting
{
    /// <summary>
    /// Gets the value of a scene variable.
    /// </summary>
    [UnitSurtitle("Scene")]
    public sealed class GetSceneVariable : GetVariableUnit, ISceneVariableUnit
    {
        public GetSceneVariable() : base() { }

        public GetSceneVariable(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            var scene = flow.stack.scene;

            if (scene == null) return null;

            return Variables.Scene(scene.Value);
        }
    }
}
