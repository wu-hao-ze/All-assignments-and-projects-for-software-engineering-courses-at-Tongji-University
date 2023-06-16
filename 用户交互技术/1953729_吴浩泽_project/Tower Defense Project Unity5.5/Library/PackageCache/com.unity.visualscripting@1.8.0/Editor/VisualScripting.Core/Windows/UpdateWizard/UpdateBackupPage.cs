using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class UpdateBackupPage : BackupPage
    {
        protected override string incentive { get; } = "Just in case anything goes wrong during the update process, we strongly suggest you create a backup of your project.";

        protected override void OnCloseButtonGUI()
        {
            if (GUILayout.Button(createdBackup ? completeLabel : "Skip", Styles.closeButton))
            {
                if (createdBackup || EditorUtility.DisplayDialog("Skip Backup", "Are you sure you want to skip backup creation?\n\nIf project data gets corrupted, there is no other way to easily recover.", "Skip", "Cancel"))
                {
                    Complete();
                }
            }
        }
    }
}
