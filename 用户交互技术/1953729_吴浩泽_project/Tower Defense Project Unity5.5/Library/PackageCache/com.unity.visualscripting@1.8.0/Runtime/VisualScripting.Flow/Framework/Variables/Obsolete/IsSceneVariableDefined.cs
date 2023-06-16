namespace Unity.VisualScripting
{
    /// <summary>
    /// Checks if a scene variable is defined.
    /// </summary>
    [UnitSurtitle("Scene")]
    public sealed class IsSceneVariableDefined : IsVariableDefinedUnit, ISceneVariableUnit
    {
        public IsSceneVariableDefined() : base() { }

        public IsSceneVariableDefined(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            var scene = flow.stack.scene;

            if (scene == null) return null;

            return Variables.Scene(scene.Value);
        }
    }
}
