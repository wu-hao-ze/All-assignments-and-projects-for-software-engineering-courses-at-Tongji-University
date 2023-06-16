using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    public sealed class BoltCoreResources : PluginResources
    {
        private BoltCoreResources(BoltCore plugin) : base(plugin)
        {
            icons = new Icons(this);
        }

        public Icons icons { get; private set; }

        public EditorTexture loader { get; private set; }

        public override void LateInitialize()
        {
            base.LateInitialize();

            icons.Load();

            loader = LoadTexture("Loader.png", CreateTextureOptions.PixelPerfect);
        }

        public class Icons
        {
            public EditorTexture variablesWindow { get; private set; }

            public EditorTexture variable { get; private set; }
            public EditorTexture flowVariable { get; private set; }
            public EditorTexture graphVariable { get; private set; }
            public EditorTexture objectVariable { get; private set; }
            public EditorTexture sceneVariable { get; private set; }
            public EditorTexture applicationVariable { get; private set; }
            public EditorTexture savedVariable { get; private set; }

            public EditorTexture window { get; private set; }
            public EditorTexture inspectorWindow { get; private set; }

            public EditorTexture empty { get; private set; }

            public EditorTexture progress { get; private set; }
            public EditorTexture errorState { get; private set; }
            public EditorTexture successState { get; private set; }
            public EditorTexture warningState { get; private set; }

            public EditorTexture informationMessage { get; private set; }
            public EditorTexture questionMessage { get; private set; }
            public EditorTexture warningMessage { get; private set; }
            public EditorTexture successMessage { get; private set; }
            public EditorTexture errorMessage { get; private set; }

            public EditorTexture upgrade { get; private set; }
            public EditorTexture upToDate { get; private set; }
            public EditorTexture downgrade { get; private set; }

            public EditorTexture supportWindow { get; private set; }
            public EditorTexture sidebarAnchorLeft { get; private set; }
            public EditorTexture sidebarAnchorRight { get; private set; }
            public EditorTexture editorPref { get; private set; }
            public EditorTexture projectSetting { get; private set; }

            public EditorTexture @null { get; private set; }

            public Icons(BoltCoreResources resources)
            {
                this.resources = resources;
            }

            private readonly BoltCoreResources resources;

            public void Load()
            {
                variablesWindow = resources.LoadIcon("VariablesWindow.png");

                variable = resources.LoadIcon("Variable.png");

                graphVariable = resources.LoadIcon("GraphVariable.png");
                objectVariable = resources.LoadIcon("ObjectVariable.png");
                sceneVariable = resources.LoadIcon("SceneVariable.png");
                applicationVariable = resources.LoadIcon("ApplicationVariable.png");
                savedVariable = resources.LoadIcon("SavedVariable.png");
                flowVariable = resources.LoadIcon("FlowVariable.png");

                window = resources.LoadIcon("GraphWindow.png");
                inspectorWindow = resources.LoadIcon("GraphInspectorWindow.png");

                if (GraphWindow.active != null)
                {
                    GraphWindow.active.titleContent.image = window?[IconSize.Small];
                }

                empty = EditorTexture.Single(ColorPalette.transparent.GetPixel());

                // Messages
                questionMessage = resources.LoadIcon("Question.png");
                warningMessage = resources.LoadIcon("Warning.png");
                successMessage = resources.LoadIcon("Success.png");
                errorMessage = resources.LoadIcon("Error.png");

                // States
                warningState = resources.LoadIcon("Warning.png");
                successState = resources.LoadIcon("Success.png");
                errorState = resources.LoadIcon("Error.png");
                progress = resources.LoadIcon("Progress.png");

                // Versioning
                upgrade = resources.LoadIcon("Upgrade.png");
                upToDate = resources.LoadIcon("UpToDate.png");
                downgrade = resources.LoadIcon("Downgrade.png");

                // Windows
                supportWindow = resources.LoadIcon("SupportWindow.png");
                sidebarAnchorLeft = resources.LoadTexture("SidebarAnchorLeft.png", CreateTextureOptions.PixelPerfect);
                sidebarAnchorRight = resources.LoadTexture("SidebarAnchorRight.png", CreateTextureOptions.PixelPerfect);

                // Configuration
                editorPref = resources.LoadTexture("EditorPref.png", new TextureResolution[] { 12, 24 }, CreateTextureOptions.PixelPerfect);
                projectSetting = resources.LoadTexture("ProjectSetting.png", new TextureResolution[] { 12, 24 }, CreateTextureOptions.PixelPerfect);

                // Other
                @null = resources.LoadIcon("Null.png");
            }

            public EditorTexture VariableKind(VariableKind kind)
            {
                switch (kind)
                {
                    case VisualScripting.VariableKind.Flow: return flowVariable;
                    case VisualScripting.VariableKind.Graph: return graphVariable;
                    case VisualScripting.VariableKind.Object: return objectVariable;
                    case VisualScripting.VariableKind.Scene: return sceneVariable;
                    case VisualScripting.VariableKind.Application: return applicationVariable;
                    case VisualScripting.VariableKind.Saved: return savedVariable;
                    default: throw new UnexpectedEnumValueException<VariableKind>(kind);
                }
            }
        }
    }
}
