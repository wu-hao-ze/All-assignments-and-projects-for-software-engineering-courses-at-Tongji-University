# Add or remove available nodes

Visual Scripting has a set of default assemblies and generated nodes for Unity features. Add more assemblies through the Visual Scripting Node Library in your Project Settings. Assemblies are special files that contain the code for the feature you want to use. 

Visual Scripting can generate nodes from assemblies in packages and third-party assets. To use a new package or third-party asset in Visual Scripting, you must import it into Unity. For more information on how to add packages to Unity, see [Adding and removing](https://docs.unity3d.com/2021.2/Documentation/Manual/upm-ui-actions.html) in the Unity User Manual. For more information on how to add third-party assets to Unity, see [Importing assets](https://docs.unity3d.com/2021.2/Documentation/Manual/ImportingAssets.html) in the User Manual.

## Add assemblies and nodes to the Node Library

 To add a new assembly and its nodes to the Node Library: 

 1. [!include[open-project-settings](./snippets/vs-open-project-settings.md)]
 1. Select **Visual Scripting**.
 1. Expand **Node Library**. 
 1. At the end of the assemblies list, select **Add** (+). 
 1. In the new assembly entry, select **(No Assembly)** to open the **Assembly** menu. 
 1. Select an available assembly from the **Assembly** menu. 

Visual Scripting adds the assembly and its nodes to the Node Library. To use the nodes in your project, [add their types to your Type Options](vs-add-remove-type-options.md#add-a-type-to-your-type-options) and [regenerate the Node Library](vs-configuration.md#Regen).

## Remove assemblies and nodes from the Node Library 

 To remove an assembly and its nodes from your Node Library: 

 1. [!include[open-project-settings](./snippets/vs-open-project-settings.md)]
 1. Select **Visual Scripting**.
 1. Expand **Node Library**. 
 1. In the assemblies list, locate the entry for the assembly you want to remove. 
 1. Select **Remove** (-). 

Visual Scripting removes the assembly and its nodes from the Node Library. To remove the nodes from the fuzzy finder and your project, [regenerate the Node Library](vs-configuration.md#Regen). You might also want to [remove their types from your Type Options](vs-add-remove-type-options.md#remove-a-type-from-your-type-options).
 