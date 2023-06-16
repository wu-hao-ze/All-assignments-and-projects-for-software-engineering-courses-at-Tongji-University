# Create and assign a graph to a new GameObject

You can use the empty graph creation flow to create a new graph file and assign it to a new GameObject. Visual Scripting automatically creates a new GameObject with the required components for the new graph file. 

![The Empty Graph Creation Flow window](images\vs-empty-graph-create-flow.png)

For more information on other ways to create a graph file, see [Create a new graph file](vs-create-graph.md).

To create a new graph and assign it to a new GameObject:

1. [!include[open-hierarchy-window](./snippets/vs-open-hierarchy-window.md)]

1. In the Hierarchy window, select the GameObject where you want to assign the new graph. 

2. [!include[visual-scripting-window](./snippets/vs-visual-scripting-window.md)]

3. Expand **Create new Script Graph**.

1. Select **on new game object**. 

4. Choose a location to save the new graph file. 

1. Enter a name for the graph. 

1. Select **Save**.  

    > [!NOTE]
    > The GameObject you create with this method has the same name as the graph file. After you have named and saved the graph file, the GameObject appears in the Hierarchy. 

    The new graph file automatically opens in a new window. 

![A new Script Graph, created with the empty graph creation flow with the On New GameObject option.](images\vs-new-graph-new-gameobject.png)

## Create a State Graph

To create a new State Graph and assign it to a new GameObject:

1. [!include[open-hierarchy-window](./snippets/vs-open-hierarchy-window.md)]

1. In the Hierarchy window, select the GameObject where you want to assign the new graph. 

2. [!include[visual-scripting-window](./snippets/vs-visual-scripting-window.md)]

3. Expand **Create new State Graph**.

1. Select **on new game object**. 

4. Choose a location to save the new graph file. 

1. Enter a name for the graph. 

1. Select **Save**.

    > [!NOTE]
    > The GameObject you create with this method has the same name as the graph file. After you have named and saved the graph file, the GameObject appears in the Hierarchy. 

    The new graph file automatically opens in a new window. 

## Next steps 

After you create your new graph, attach it to a Script Machine or State Machine to use it in your application. For more information, see [Attach a graph file to a Script Machine or State Machine](vs-attach-graph-machine.md).
