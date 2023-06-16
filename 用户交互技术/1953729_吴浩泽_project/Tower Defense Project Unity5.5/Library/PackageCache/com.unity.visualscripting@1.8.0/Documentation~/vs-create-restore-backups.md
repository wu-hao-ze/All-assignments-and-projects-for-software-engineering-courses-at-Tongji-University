# Create or restore a backup 

If you don't use a version control system, such as [Unity Collaborate](https://docs.unity3d.com/Manual/UnityCollaborate.html), [Git](https://git-scm.com/), or [Subversion](https://subversion.apache.org/), it's a good practice to create backups of your Visual Scripting assets and settings. Create a backup at any time from your [Project Settings](vs-configuration.md). 

Back up your data before you update Visual Scripting to a new version. For more information on the update process, see [Update Visual Scripting](vs-update.md).

## Create a new backup 

To create a new backup of your Visual Scripting assets and settings: 

1. [!include[open-project-settings](./snippets/vs-open-project-settings.md)]
1. Select **Visual Scripting**.
1. Select **Create Backup**, then select **OK**. 

Visual Scripting creates a .zip file, with a name in the format `Assets_YYYY_MM_DD_HH_MM_SS`, in a `Backups` folder inside the Unity Project.

## Restore an existing backup 

To restore an existing backup of your Visual Scripting assets and settings: 

1. [!include[open-project-settings](./snippets/vs-open-project-settings.md)] 
1. Select **Visual Scripting**.
1. Select **Restore Backup**. 

Visual Scripting opens your `Backups` folder in your system's file explorer. You can extract a .zip back-up file and import graphs and settings back into Unity. 
    
For more information on how to import assets into Unity, see [Importing assets](https://docs.unity3d.com/Manual/ImportingAssets.html) in the Unity User Manual.