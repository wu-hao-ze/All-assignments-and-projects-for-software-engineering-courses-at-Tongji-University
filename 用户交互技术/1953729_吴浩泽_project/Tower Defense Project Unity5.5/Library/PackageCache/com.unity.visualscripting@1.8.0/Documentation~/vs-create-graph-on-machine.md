# Create a graph on a Script Machine or State Machine
    
You can create a new graph file directly from a Script Machine or State Machine component on a GameObject. For more information on how to create a Script Machine or State Machine, see [Attach a graph file to a Script Machine or State Machine](vs-attach-graph-machine.md). 

## Create a new graph file from a Script Machine or State Machine

To create a new graph file from an existing Script Machine or State Machine: 

1. [!include[open-hierarchy-window](./snippets/vs-open-hierarchy-window.md)]

1. In the Hierarchy window, select a GameObject that has a Script Machine or State Machine. 

2. [!include[open-inspector-window](./snippets/vs-open-inspector-window.md)] 

3. In the Inspector window, on your Script Machine or State Machine component, set the **Source** set to **Graph**. 

1. Select **New**.

4. Enter a name for your new graph file.

5. Choose a location for the file in your project. 

1. Select **Save**.

![A new Script Machine with an attached Script Graph](images\vs-script-machine.png)

## Create a new embedded graph on a Script Machine or State Machine 

You can create an embedded graph on a Script Machine or State Machine component instead of  an external graph file: 

1. [!include[open-hierarchy-window](./snippets/vs-open-hierarchy-window.md)]

1. In the Hierarchy window, select a GameObject that has a Script Machine or State Machine. 

2. [!include[open-inspector-window](./snippets/vs-open-inspector-window.md)] 

3. In the Inspector window, on your Script Machine or State Machine component, set the **Source** to **Embed**. 

4. (Optional) In the **(Title)** field, enter a descriptive title for the embedded graph. 

5. (Optional) In the **(Summary)** field, enter a brief summary of what the embedded graph does. 

6. (Optional) To open the new embedded graph and edit, select **Edit Graph**. 

> [!NOTE]
> Unity recommends you create a graph file rather than an embedded graph. In some situations, an embedded graph works best. For more information on how to choose the correct graph type, see [Source types for Script Machines and State Machines](vs-graph-machine-types.md#source-types). 

## Next steps 

After you attach a graph to a Script Machine or State Machine, you can open the graph and edit. For more information, see [Open a graph file](vs-open-graph-edit.md).
