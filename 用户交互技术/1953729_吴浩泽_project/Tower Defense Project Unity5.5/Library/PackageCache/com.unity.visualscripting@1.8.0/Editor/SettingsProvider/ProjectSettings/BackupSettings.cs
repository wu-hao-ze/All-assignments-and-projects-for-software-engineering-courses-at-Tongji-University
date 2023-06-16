using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class BackupSettings
    {
        private const string Title = "Backup Graphs";
        private const string ButtonBackupLabel = "Create Backup";
        private const string ButtonRestoreLabel = "Restore Backup";

        public void OnGUI()
        {
            GUILayout.Space(5f);

            GUILayout.Label(Title, EditorStyles.boldLabel);

            GUILayout.Space(5f);

            if (GUILayout.Button(ButtonBackupLabel, Styles.defaultsButton))
            {
                VSBackupUtility.Backup();

                EditorUtility.DisplayDialog("Backup", "Backup completed successfully.", "OK");
            }

            if (GUILayout.Button(ButtonRestoreLabel, Styles.defaultsButton))
            {
                PathUtility.CreateDirectoryIfNeeded(Paths.backups);
                Process.Start(Paths.backups);
            }
        }

        private static class Styles
        {
            static Styles()
            {
                defaultsButton = new GUIStyle("Button");
                defaultsButton.padding = new RectOffset(10, 10, 4, 4);
            }

            public static readonly GUIStyle defaultsButton;
        }
    }
}
