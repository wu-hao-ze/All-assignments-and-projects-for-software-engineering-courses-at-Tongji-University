# Configure project settings

> [!NOTE]
> To use Visual Scripting in a project for the first time, you must [initialize it](#Initialize) from the Editor's [Project Settings](https://docs.unity3d.com/Manual/comp-ManagerGroup.html) window. 

Use the Project Settings window with Visual Scripting to manage backups, node assemblies, type options, and regenerate your Node Library.

To open your Project Settings: 

1. [!include[open-project-settings](./snippets/vs-open-project-settings.md)]
1. Select **Visual Scripting**.  

You can find the following configuration options in your Visual Scripting Project Settings. To use Visual Scripting in a project for the first time, you must [regenerate your Node Library](#Regen), as described in the table below. 

<table>
<thead>
<tr>
<th><strong>Option</strong></th>
<th>Description</th>
</tr>
</thead>
<tbody>
<tr>
<td><strong><a name="Initialize">Initialize Visual Scripting</a></strong></td>
<td>You must select <strong>Initialize Visual Scripting</strong> the first time you use Visual Scripting in a project. Initialize Visual Scripting to parse all assemblies and types for the Visual Scripting Node Library. After you initialize Visual Scripting, regenerate your Node Library. See <strong><a href="#Regen">Regenerate Nodes</a></strong>, below.</td>
</tr>
<tr>
<td><strong>Type Options</strong></td>
<td>Use the Type Options list to add or remove types for your node inputs and outputs. After you add or remove a type, you must regenerate your Node Library. See <strong><a href="#Regen">Regenerate Nodes</a></strong>, below. <br/>For more information on how to add or remove types, see <a href="vs-add-remove-type-options.md">Add or remove types</a>.</td>
</tr>
<tr>
<td><strong>Node Library</strong></td>
<td>Use the Node Library list to add or remove nodes and their assemblies in Visual Scripting. You must add any new types to your Type Options after you add new nodes to Visual Scripting. You must also regenerate your Node Library after you add or remove nodes. See <a href="#Regen"><strong>Regenerate Nodes</strong></a>, below. <br/>For more information on how to add or remove nodes from your Node Library, see <a href="vs-add-remove-node-library.md">Add or remove available nodes</a>.</td>
</tr>
<tr>
<td><strong><a name="Regen">Regenerate Nodes</a></strong></td>
<td>Regenerate your Node Library to make all nodes available for use in a project. <br/>To use Visual Scripting for the first time in a project, you must <strong><a href="#Initialize">Initialize Visual Scripting</a></strong> and regenerate your Node Library. <br/>To regenerate your Node Library: 
<ol>
    <li><p>Select <strong>Regenerate Nodes</strong>.</p></li>
    <li><p>Select <strong>OK</strong>.</p></li>
</ol>
<div class="NOTE"><h5>NOTE</h5><p>You must regenerate your Node Library in the following circumstances: 
<ul>
<li>Before you use Visual Scripting in your project for the first time.</li>
<li>After you add or remove nodes from your Node Library.</li>
<li>After you add or remove types from your Type Options.</li>
<li>After you change the inputs or outputs for a Custom C# node.</li>
</ul></p></div></td>
</tr>
<tr>
<td><strong>Generate</strong></td>
<td>To generate required property provider scripts for custom drawers, select <strong>Generate</strong>. <br/>These scripts are necessary for Unity to use custom drawers for custom classes and script variables inside Visual Scripting. To assign a default value to a custom variable type through the Unity Editor’s Inspector, you must either have access to the source code for the class, or provide a custom PropertyDrawer. For more information, see <a href="vs-custom-types.md">Custom types</a>.</td>
</tr>
<tr>
<td><strong>Create Backup</strong></td>
<td>To create a new backup of your Visual Scripting graphs and settings, select <strong>Create Backup</strong>. <br/> For more information about backups, see <a href="vs-create-restore-backups.md">Create or restore a backup</a>.</td>
</tr>
<tr>
<td><strong>Restore Backup</strong></td>
<td>To open the folder where Visual Scripting stores your backups, select <strong>Restore Backup</strong>. <br/>For more information about backups, see <a href="vs-create-restore-backups.md">Create or restore a backup</a>.</td>
</tr>
<tr>
<td><strong>Fix Missing Scripts</strong></td>
<td>To correct any issues that might occur after migration from the Unity Asset Store version of Visual Scripting to the package version, select <strong>Fix Missing Scripts</strong>. This resolves any missing references to Visual Scripting Script Graphs and State Graphs in Script Machine or State Machine components.</td>
</tr>
</tbody>
</table>

>[!NOTE] 
> If your settings don't apply after you make a change, [report a bug through the Unity Editor](https://unity3d.com/unity/qa/bug-reporting).
