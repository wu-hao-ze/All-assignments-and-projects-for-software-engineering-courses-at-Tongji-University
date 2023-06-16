namespace Unity.VisualScripting
{
    public struct ScriptReferenceReplacement
    {
        public ScriptReference previousReference;

        public ScriptReference newReference;

        public ScriptReferenceReplacement(ScriptReference previousReference, ScriptReference newReference)
        {
            this.previousReference = previousReference;
            this.newReference = newReference;
        }

        public static ScriptReferenceReplacement From<T>(ScriptReference previous)
        {
            return new ScriptReferenceReplacement(previous, ScriptReference.Existing(typeof(T)));
        }

        public static ScriptReferenceReplacement FromDll<T>(string dllGuid)
        {
            return new ScriptReferenceReplacement(ScriptReference.Dll(dllGuid, typeof(T)), ScriptReference.Existing(typeof(T)));
        }

        public static ScriptReferenceReplacement FromCs<T>(string csGuid)
        {
            return new ScriptReferenceReplacement(ScriptReference.Cs(csGuid), ScriptReference.Existing(typeof(T)));
        }

        public override string ToString()
        {
            return previousReference + " => " + newReference;
        }
    }
}
