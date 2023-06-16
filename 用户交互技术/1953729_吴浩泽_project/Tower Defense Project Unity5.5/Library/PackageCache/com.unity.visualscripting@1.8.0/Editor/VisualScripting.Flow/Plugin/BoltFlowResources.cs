using System.Collections.Generic;
using UnityEditor;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    public sealed class BoltFlowResources : PluginResources
    {
        private BoltFlowResources(BoltFlow plugin) : base(plugin)
        {
            icons = new Icons(this);
        }

        public Icons icons { get; private set; }


        public override void LateInitialize()
        {
            base.LateInitialize();

            icons.Load();
        }

        public class Icons
        {
            public Icons(BoltFlowResources resources)
            {
                this.resources = resources;
            }

            private readonly Dictionary<UnitCategory, EditorTexture> unitCategoryIcons = new Dictionary<UnitCategory, EditorTexture>();

            private readonly BoltFlowResources resources;

            public EditorTexture graph { get; private set; }
            public EditorTexture unit { get; private set; }
            public EditorTexture flowMacro { get; private set; }
            public EditorTexture unitCategory { get; private set; }

            public EditorTexture controlPortConnected { get; private set; }
            public EditorTexture controlPortUnconnected { get; private set; }
            public EditorTexture valuePortConnected { get; private set; }
            public EditorTexture valuePortUnconnected { get; private set; }
            public EditorTexture invalidPortConnected { get; private set; }
            public EditorTexture invalidPortUnconnected { get; private set; }

            public EditorTexture coroutine { get; private set; }

            public void Load()
            {
                graph = typeof(FlowGraph).Icon();
                unit = typeof(IUnit).Icon();
                flowMacro = resources.LoadIcon("FlowMacro.png");
                unitCategory = resources.LoadIcon("UnitCategory.png");

                var portResolutions = new[] { new TextureResolution(9, 12), new TextureResolution(12, 24) };
                var portOptions = CreateTextureOptions.PixelPerfect;

                controlPortConnected = resources.LoadTexture("ControlPortConnected.png", portResolutions, portOptions);
                controlPortUnconnected = resources.LoadTexture("ControlPortUnconnected.png", portResolutions, portOptions);
                valuePortConnected = resources.LoadTexture("ValuePortConnected.png", portResolutions, portOptions);
                valuePortUnconnected = resources.LoadTexture("ValuePortUnconnected.png", portResolutions, portOptions);
                invalidPortConnected = resources.LoadTexture("InvalidPortConnected.png", portResolutions, portOptions);
                invalidPortUnconnected = resources.LoadTexture("InvalidPortUnconnected.png", portResolutions, portOptions);

                coroutine = resources.LoadIcon("Coroutine.png");
            }

            public EditorTexture UnitCategory(UnitCategory category)
            {
                if (category == null)
                {
                    return unitCategory;
                }

                if (!unitCategoryIcons.ContainsKey(category))
                {
                    var path = $"{category.name}.png";

                    EditorTexture editorTexture = LoadSharedIcon(path, false);

                    if (editorTexture == null)
                    {
                        editorTexture = unitCategory;
                    }

                    unitCategoryIcons.Add(category, editorTexture);
                }

                return unitCategoryIcons[category];
            }
        }
    }
}
