using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class GenerateDocumentationPage : Page
    {
        public GenerateDocumentationPage()
        {
            title = "Generate Documentation";
            shortTitle = "Documentation";
            icon = BoltCore.Resources.LoadIcon("GenerateDocumentationPage.png");
        }

        private readonly List<DocumentationGenerationStep> steps = new List<DocumentationGenerationStep>();

        private readonly Queue<DocumentationGenerationStep> queue = new Queue<DocumentationGenerationStep>();

        private bool generating;

        protected override void OnShow()
        {
            Paths.SyncUnitySolution();

            steps.Clear();

            if (Paths.runtimeAssemblyFirstPassProject != null)
            {
                steps.Add(new DocumentationGenerationStep(Paths.runtimeAssemblyFirstPassProject, "Runtime (First Pass)"));
            }

            if (Paths.runtimeAssemblySecondPassProject != null)
            {
                steps.Add(new DocumentationGenerationStep(Paths.runtimeAssemblySecondPassProject, "Runtime (Second Pass)"));
            }

            if (Paths.editorAssemblyFirstPassProject != null)
            {
                steps.Add(new DocumentationGenerationStep(Paths.editorAssemblyFirstPassProject, "Editor (First Pass)"));
            }

            if (Paths.editorAssemblySecondPassProject != null)
            {
                steps.Add(new DocumentationGenerationStep(Paths.editorAssemblySecondPassProject, "Editor (Second Pass)"));
            }

            queue.Clear();

            if (steps.Count == 0)
            {
                Complete();
            }
        }

        public override void Update()
        {
            base.Update();

            foreach (var query in steps)
            {
                query.Update();
            }

            if (queue.Count > 0 &&
                (queue.Peek().state == DocumentationGenerationStep.State.Success ||
                 queue.Peek().state == DocumentationGenerationStep.State.Failure))
            {
                queue.Dequeue();

                if (queue.Count > 0)
                {
                    queue.Peek().Generate();
                }
                else
                {
                    generating = false;
                    Selection.activeObject = null;
                }
            }
        }

        protected override void OnContentGUI()
        {
            var previousIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(IconSize.Small, IconSize.Small));

            var explanation = "Bolt plugins can automatically display documentation for Unity methods in graphs and in the inspector. ";
            explanation += "To also include documentation from your custom code and from third-party plugins, we need to generate it first.";

            GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));

            LudiqGUI.FlexibleSpace();
            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();
            GUILayout.Label(explanation, Styles.explanationLabel, GUILayout.MaxWidth(350));
            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();
            LudiqGUI.FlexibleSpace();

            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(queue.Count > 0);

            if (GUILayout.Button("Generate Documentation", Styles.generateButton))
            {
                foreach (var step in steps)
                {
                    step.Reset();
                }

                foreach (var step in steps)
                {
                    queue.Enqueue(step);
                }

                queue.Peek().Generate();
                generating = true;
            }

            EditorGUI.EndDisabledGroup();

            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();

            LudiqGUI.FlexibleSpace();

            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();
            LudiqGUI.BeginVertical();

            foreach (var step in steps)
            {
                step.OnGUI();

                LudiqGUI.Space(Styles.spaceBetweenSteps);
            }

            LudiqGUI.EndVertical();
            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();

            LudiqGUI.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(generating);
            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();

            if (GUILayout.Button(completeLabel, Styles.nextButton))
            {
                Complete();
            }

            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            LudiqGUI.FlexibleSpace();
            GUILayout.Label("You can regenerate documentation at any time from the tools menu.", EditorStyles.centeredGreyMiniLabel);

            LudiqGUI.EndVertical();

            EditorGUIUtility.SetIconSize(previousIconSize);
        }

        public static class Styles
        {
            static Styles()
            {
                background = new GUIStyle(LudiqStyles.windowBackground);
                background.padding = new RectOffset(10, 10, 10, 16);

                explanationLabel = new GUIStyle(LudiqStyles.centeredLabel);

                generateButton = new GUIStyle("Button");
                generateButton.padding = new RectOffset(16, 16, 8, 8);

                nextButton = new GUIStyle("Button");
                nextButton.padding = new RectOffset(10, 10, 6, 6);

                stepIcon = new GUIStyle();
                stepIcon.fixedWidth = IconSize.Small;
                stepIcon.fixedHeight = IconSize.Small;
                stepIcon.margin.right = 5;

                stepLabel = new GUIStyle(EditorStyles.label);
                stepLabel.alignment = TextAnchor.MiddleLeft;
                stepLabel.padding = new RectOffset(0, 0, 0, 0);
                stepLabel.margin = new RectOffset(0, 0, 0, 0);
                stepLabel.fixedHeight = stepIcon.fixedHeight;

                stepIdleIcon = new GUIStyle(stepLabel);
                stepIdleIcon.normal.textColor = ColorPalette.unityForegroundDim;

                stepShowLogButton = new GUIStyle(stepLabel);
                stepShowLogButton.normal.textColor = ColorPalette.hyperlink;
                stepShowLogButton.active.textColor = ColorPalette.hyperlinkActive;

                stepShowLogHiddenButton = new GUIStyle(stepLabel);
                stepShowLogHiddenButton.normal.textColor = new Color(0, 0, 0, 0);
            }

            public static readonly GUIStyle background;

            public static readonly GUIStyle explanationLabel;

            public static readonly GUIStyle generateButton;

            public static readonly GUIStyle nextButton;

            public static readonly GUIStyle stepLabel;

            public static readonly GUIStyle stepIdleIcon;

            public static readonly GUIStyle stepShowLogButton;

            public static readonly GUIStyle stepShowLogHiddenButton;

            public static readonly GUIStyle stepIcon;

            public static readonly float spaceBetweenSteps = 5;
        }

        private class DocumentationGenerationStep
        {
            public enum State
            {
                Idle,

                Generating,

                Success,

                Failure
            }

            public DocumentationGenerationStep(string projectPath, string label)
            {
                state = State.Idle;
                this.projectPath = projectPath;
                this.label = label;
            }

            private readonly string projectPath;

            private readonly string label;

            private State lastDrawnState;

            private string logPath;

            public State state { get; private set; }

            private EditorTexture GetStateIcon(State state)
            {
                switch (state)
                {
                    case State.Idle:
                        return BoltCore.Icons.empty;
                    case State.Generating:
                        return BoltCore.Icons.progress;
                    case State.Success:
                        return BoltCore.Icons.successState;
                    case State.Failure:
                        return BoltCore.Icons.errorState;
                    default:
                        throw new UnexpectedEnumValueException<State>(state);
                }
            }

            public void Generate()
            {
                state = State.Generating;
            }

            public void Reset()
            {
                state = State.Idle;
                logPath = null;
                lastDrawnState = state;
            }

            public void Update()
            {
                if (lastDrawnState == State.Generating)
                {
                    logPath = Path.GetTempPath() + Guid.NewGuid() + ".txt";

                    try
                    {
                        File.WriteAllText(logPath, DocumentationGenerator.GenerateDocumentation(projectPath));
                        state = State.Success;
                    }
                    catch (Exception ex)
                    {
                        state = State.Failure;
                        File.WriteAllText(logPath, ex.ToString());
                    }
                }
            }

            public void OnGUI()
            {
                LudiqGUI.BeginHorizontal();

                GUILayout.Box(GetStateIcon(state)?[IconSize.Small], Styles.stepIcon);

                GUILayout.Label(label, state == State.Idle ? Styles.stepIdleIcon : Styles.stepLabel, GUILayout.ExpandWidth(false));

                LudiqGUI.Space(5);

                if (logPath != null)
                {
                    if (GUILayout.Button("Show Log", Styles.stepShowLogButton, GUILayout.ExpandWidth(false)))
                    {
                        Process.Start(logPath);
                    }
                }
                else
                {
                    GUILayout.Label("Show Log", Styles.stepShowLogHiddenButton, GUILayout.ExpandWidth(false));
                }

                LudiqGUI.EndHorizontal();

                if (Event.current.type == EventType.Repaint)
                {
                    lastDrawnState = state;
                }
            }
        }
    }
}
