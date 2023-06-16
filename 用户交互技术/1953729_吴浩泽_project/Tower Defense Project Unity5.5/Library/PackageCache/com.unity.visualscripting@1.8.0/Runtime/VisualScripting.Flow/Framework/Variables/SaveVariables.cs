namespace Unity.VisualScripting
{
    /// <summary>
    /// Forces saved variables to be saved to the PlayerPrefs.
    /// This is useful on WebGL where automatic save on quit is not supported.
    /// </summary>
    [UnitCategory("Variables")]
    public sealed class SaveVariables : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);
            exit = ControlOutput(nameof(exit));

            Succession(enter, exit);
        }

        private ControlOutput Enter(Flow arg)
        {
            SavedVariables.SaveDeclarations(SavedVariables.merged);
            return exit;
        }
    }
}
