namespace Unity.VisualScripting
{
    public abstract class UnitPortDefinition : IUnitPortDefinition
    {
        [Serialize, Inspectable, InspectorDelayed]
        [WarnBeforeEditing("Edit Port Key", "Changing the key of this definition will break any existing connection to this port. Are you sure you want to continue?", null, "")]
        public string key { get; set; }

        [Serialize, Inspectable]
        public string label { get; set; }

        [Serialize, Inspectable, InspectorTextArea]
        public string summary { get; set; }

        [Serialize, Inspectable]
        public bool hideLabel { get; set; }

        [DoNotSerialize]
        public virtual bool isValid => !string.IsNullOrEmpty(key);
    }
}
