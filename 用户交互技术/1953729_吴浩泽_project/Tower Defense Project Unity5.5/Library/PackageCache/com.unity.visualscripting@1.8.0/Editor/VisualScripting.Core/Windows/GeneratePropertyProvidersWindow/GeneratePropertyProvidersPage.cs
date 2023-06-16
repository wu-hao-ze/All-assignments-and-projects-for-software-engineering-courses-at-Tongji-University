using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class GeneratePropertyProvidersPage : Page
    {
        public GeneratePropertyProvidersPage()
        {
            title = $"Generate Custom Inspectors";
            shortTitle = "Inspectors";
            icon = BoltCore.Resources.LoadIcon("GeneratePropertyProvidersPage.png");
        }

        protected override void OnContentGUI()
        {
            GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));

            var label = "Inspectors in Bolt plugins can handle many custom types besides Unity primites and objects. ";
            label += "However, to be compatible with your custom editor drawers, some additional property provider scripts must be generated. ";

            LudiqGUI.FlexibleSpace();
            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();
            GUILayout.Label(label, LudiqStyles.centeredLabel, GUILayout.MaxWidth(350));
            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();
            LudiqGUI.FlexibleSpace();

            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();

            if (GUILayout.Button("Generate Inspectors", Styles.nextButton))
            {
                try
                {
                    SerializedPropertyProviderProvider.instance.GenerateProviderScripts();
                    // EditorUtility.DisplayDialog("Custom Inspector Generation", "Custom inspector generation has completed successfully.", "OK");
                    Complete();
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("Custom Inspector Error", $"Custom inspector generation has failed: \n{ex.Message}", "OK");
                    Debug.LogException(ex);
                }
            }

            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();

            LudiqGUI.FlexibleSpace();

            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();

            if (GUILayout.Button(completeLabel, Styles.skipButton))
            {
                Complete();
            }

            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();

            LudiqGUI.FlexibleSpace();
            GUILayout.Label("You can regenerate inspectors at any time from the tools menu.", Styles.regenerateLabel);

            LudiqGUI.FlexibleSpace();

            LudiqGUI.EndVertical();
        }

        public static class Styles
        {
            static Styles()
            {
                background = new GUIStyle(LudiqStyles.windowBackground);
                background.padding = new RectOffset(30, 30, 10, 16);

                nextButton = new GUIStyle("Button");
                nextButton.padding = new RectOffset(20, 20, 10, 10);

                skipButton = new GUIStyle("Button");
                skipButton.padding = new RectOffset(10, 10, 6, 6);

                regenerateLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                regenerateLabel.wordWrap = true;
            }

            public static readonly GUIStyle background;
            public static readonly GUIStyle nextButton;
            public static readonly GUIStyle skipButton;
            public static readonly GUIStyle regenerateLabel;
        }
    }
}
