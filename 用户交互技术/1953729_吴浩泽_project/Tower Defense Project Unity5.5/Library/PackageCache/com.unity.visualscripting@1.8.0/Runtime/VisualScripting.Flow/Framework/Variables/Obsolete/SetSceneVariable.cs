namespace Unity.VisualScripting
{
    /// <summary>
    /// Sets the value of a scene variable.
    /// </summary>
    [UnitSurtitle("Scene")]
    public sealed class SetSceneVariable : SetVariableUnit, ISceneVariableUnit
    {
        public SetSceneVariable() : base() { }

        public SetSceneVariable(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            var scene = flow.stack.scene;

            if (scene == null) return null;

            return Variables.Scene(scene.Value);
        }
    }
}
