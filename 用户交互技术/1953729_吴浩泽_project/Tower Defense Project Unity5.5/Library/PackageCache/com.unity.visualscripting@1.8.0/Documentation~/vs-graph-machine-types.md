# Script Machines and State Machines

A Script Machine is a GameObject component that lets you use a Script Graph in an application. You can't use a Script Graph unless it's attached to a Script Machine. For more information on components, see [Introduction to components](https://docs.unity3d.com/Manual/Components.html) in the Unity User Manual. 

Script Machines can either link to a graph asset, or they can contain an embedded Script Graph asset.

![An image of the Unity Inspector that displays a blank Script Machine component on a GameObject.](images\vs-script-machine-blank.png)

A State Machine is the same as a Script Machine, except it contains a State Graph.

![An image of the Unity Inspector that displays a blank State Machine component on a GameObject.](images\vs-state-machine-blank.png)

For more information on the difference between a Script Graph and a State Graph, see [Graphs](vs-graph-types.md).

Add a Script Machine or State Machine component to a GameObject and Visual Scripting automatically adds a Variables component. The Variables component holds any Object variables that you define in a Script Graph or State Graph attached to the GameObject. For more information on variables, see [Variables](vs-variables.md).

For more information on how to add a Script Machine or State Machine to a GameObject and attach a graph file, see [Attach a graph file to a Script Machine or State Machine](vs-attach-graph-machine.md)

## Source types

Script Machines and State Machines have two options for their Source: a graph file (**Graph**), or an embedded asset (**Embed**). 

Set the **Source** for a Script Machine or State Machine at any time. If you switch the Source from **Graph** to **Embed**, the graph file still exists as a separate file from the State Machine or Script Machine inside of your project.

> [!CAUTION]
> If you switch your Source from **Embed** to **Graph**, you will lose the embedded graph asset. You can copy the nodes from an embedded graph to a graph asset to avoid data loss. 

Other features of Visual Scripting, such as transitions, Super States, and Subgraphs, also have these source type options.

### The Graph source type 

Use the **Graph** source type to make your graphs faster to load and easier to maintain. Any changes made to a graph file apply to every Script Machine or State Machine that links to that graph file, even if those GameObjects don't use the same Prefab. 

To use the same graph across multiple GameObjects, use a **Graph** source type. You might encounter some situations where an embedded graph works best. 

### The Embed source type 

An **Embed** graph exists only in the scene where it's created, if it isn't attached to a Prefab. This can cause problems with source control systems. 

If you delete a GameObject with an **Embed** graph asset, you will lose your graph. Changes made to an embedded graph aren't saved while the Editor is in Play mode.

Use the **Embed** source type if: 

- You need references to GameObjects from the current scene in the graph and the graph isn't on a Prefab. 
- The graph is on a Prefab that you plan to instantiate in the application during runtime. 
- You only need to use the logic in the graph once in the application. 

You can't reuse an embedded graph across multiple GameObjects unless the graph is on a Prefab. An embed graph only exists on the Script Machine or State Machine where you created it. This means you can share the graph across instances of a Prefab, but not on more than one GameObject. For more information about Prefabs, see [Prefabs](https://docs.unity3d.com/Manual/Prefabs.html) in the User Manual.
