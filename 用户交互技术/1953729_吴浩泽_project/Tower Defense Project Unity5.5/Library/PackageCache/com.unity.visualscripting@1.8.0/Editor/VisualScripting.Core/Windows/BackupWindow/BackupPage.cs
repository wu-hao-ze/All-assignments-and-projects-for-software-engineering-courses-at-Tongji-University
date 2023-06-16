using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class BackupPage : Page
    {
        public BackupPage() : base()
        {
            title = "Backup";
            shortTitle = "Backup";
            icon = BoltCore.Resources.LoadIcon("BackupPage.png");
        }

        protected bool createdBackup { get; private set; }

        protected virtual string incentive { get; } = "We strongly suggest that you create frequent backups of your project if you don't use version control.";

        protected virtual void OnCloseButtonGUI()
        {
            if (GUILayout.Button(completeLabel, Styles.closeButton))
            {
                Complete();
            }
        }

        protected override void OnContentGUI()
        {
            GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));

            LudiqGUI.FlexibleSpace();
            LudiqGUI.FlexibleSpace();

            var paragraph = incentive;

            paragraph += " Here, you can create a zip of your assets folder automatically and store it under 'Project\u00a0/\u00a0Backups'.)";

            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();
            GUILayout.Label(paragraph, LudiqStyles.centeredLabel, GUILayout.MaxWidth(370));
            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();

            LudiqGUI.FlexibleSpace();

            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();

            if (GUILayout.Button("Create Backup", Styles.createBackupButton))
            {
                try
                {
                    BackupUtility.BackupAssetsFolder();
                    createdBackup = true;
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("Backup Error", "Failed to create backup:\n\n" + ex.Message, "OK");
                }
            }

            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();

            LudiqGUI.Space(10);

            if (createdBackup)
            {
                GUILayout.Label(new GUIContent(" Backup created", BoltCore.Icons.successState?[IconSize.Small]), Styles.backupCreatedLabel);
            }
            else
            {
                LudiqGUI.Space(Styles.backupCreatedLabel.fixedHeight + 2);
            }

            LudiqGUI.Space(10);

            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();

            OnCloseButtonGUI();

            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();

            LudiqGUI.FlexibleSpace();
            LudiqGUI.FlexibleSpace();

            LudiqGUI.EndVertical();
        }

        public static class Styles
        {
            static Styles()
            {
                background = new GUIStyle(LudiqStyles.windowBackground);
                background.padding = new RectOffset(20, 20, 10, 10);

                createBackupButton = new GUIStyle("Button");
                createBackupButton.padding = new RectOffset(20, 20, 10, 10);

                backupCreatedLabel = new GUIStyle(LudiqStyles.centeredLabel);
                backupCreatedLabel.fixedHeight = IconSize.Small;
                backupCreatedLabel.margin = new RectOffset();
                backupCreatedLabel.padding = new RectOffset();

                closeButton = new GUIStyle("Button");
                closeButton.padding = new RectOffset(12, 12, 7, 7);
            }

            public static readonly GUIStyle background;
            public static readonly GUIStyle createBackupButton;
            public static readonly GUIStyle backupCreatedLabel;
            public static readonly GUIStyle closeButton;
        }
    }
}
