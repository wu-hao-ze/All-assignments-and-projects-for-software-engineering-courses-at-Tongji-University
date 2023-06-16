using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class NamingSchemePage : Page
    {
        public NamingSchemePage()
        {
            title = "Naming Scheme";
            shortTitle = "Naming";
            icon = BoltCore.Resources.LoadIcon("NamingSchemePage.png");
        }

        protected override void OnContentGUI()
        {
            var previousIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(IconSize.Small, IconSize.Small));

            GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));

            LudiqGUI.FlexibleSpace();
            GUILayout.Label("How do you want names to be displayed?", LudiqStyles.centeredLabel);
            LudiqGUI.FlexibleSpace();
            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();

            LudiqGUI.BeginVertical();

            if (GUILayout.Button("Human Naming", Styles.modeButton))
            {
                BoltCore.Configuration.humanNaming = true;
                BoltCore.Configuration.Save();
                Complete();
            }

            LudiqGUI.Space(-1);
            GUILayout.BeginVertical(Styles.modeBox);
            GUILayout.Label(new GUIContent(" Transform: Get Position", typeof(Transform).Icon()?[IconSize.Small]), Styles.example);
            GUILayout.Label(new GUIContent(" Integer", typeof(int).Icon()?[IconSize.Small]), Styles.example);
            GUILayout.Label(new GUIContent(" List of Game Object", typeof(List<GameObject>).Icon()?[IconSize.Small]), Styles.example);
            GUILayout.Label(new GUIContent(" Rigidbody: Add Force (Force)", typeof(Rigidbody).Icon()?[IconSize.Small]), Styles.example);
            LudiqGUI.EndVertical();
            LudiqGUI.EndVertical();

            LudiqGUI.Space(10);

            LudiqGUI.BeginVertical();

            if (GUILayout.Button("Programmer Naming", Styles.modeButton))
            {
                BoltCore.Configuration.humanNaming = false;
                BoltCore.Configuration.Save();
                Complete();
            }

            LudiqGUI.Space(-1);
            GUILayout.BeginVertical(Styles.modeBox);
            GUILayout.Label(new GUIContent(" Transform.position (Get)", typeof(Transform).Icon()?[IconSize.Small]), Styles.example);
            GUILayout.Label(new GUIContent(" int", typeof(int).Icon()?[IconSize.Small]), Styles.example);
            GUILayout.Label(new GUIContent(" List<GameObject>", typeof(List<GameObject>).Icon()?[IconSize.Small]), Styles.example);
            GUILayout.Label(new GUIContent(" Rigidbody.AddForce(force)", typeof(Rigidbody).Icon()?[IconSize.Small]), Styles.example);
            LudiqGUI.EndVertical();
            LudiqGUI.EndVertical();

            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();
            LudiqGUI.FlexibleSpace();
            GUILayout.Label("You can change this setting at any time from the setup or node options wizard.", EditorStyles.centeredGreyMiniLabel);
            LudiqGUI.FlexibleSpace();

            LudiqGUI.EndVertical();

            EditorGUIUtility.SetIconSize(previousIconSize);
        }

        public static class Styles
        {
            static Styles()
            {
                background = new GUIStyle(LudiqStyles.windowBackground);
                background.padding = new RectOffset(10, 10, 10, 16);

                modeButton = new GUIStyle("Button");
                modeButton.padding = new RectOffset(6, 6, 6, 6);
                modeButton.margin = new RectOffset(0, 0, 0, 0);

                modeBox = new GUIStyle("TextField");
                modeBox.fixedHeight = 0;
                modeBox.margin = new RectOffset(0, 0, 0, 0);
                modeBox.padding = new RectOffset(5, 7, 8, 7);

                example = new GUIStyle(EditorStyles.label);
            }

            public static readonly GUIStyle background;
            public static readonly GUIStyle modeButton;
            public static readonly GUIStyle modeBox;
            public static readonly GUIStyle example;
        }
    }
}
