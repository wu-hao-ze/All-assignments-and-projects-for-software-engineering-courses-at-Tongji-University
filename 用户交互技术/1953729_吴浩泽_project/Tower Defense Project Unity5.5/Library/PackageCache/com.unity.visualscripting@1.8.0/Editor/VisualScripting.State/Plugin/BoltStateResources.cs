namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    public sealed class BoltStateResources : PluginResources
    {
        private BoltStateResources(BoltState plugin) : base(plugin)
        {
            icons = new Icons(this);
        }

        public Icons icons { get; private set; }

        public override void LateInitialize()
        {
            icons.Load();
        }

        public class Icons
        {
            public Icons(BoltStateResources resources)
            {
                this.resources = resources;
            }

            private readonly BoltStateResources resources;
            public EditorTexture graph { get; private set; }
            public EditorTexture state { get; private set; }

            public void Load()
            {
                graph = typeof(StateGraph).Icon();
                state = typeof(State).Icon();
            }
        }
    }
}
