# Create and assign a graph to an existing GameObject

You can use the empty graph creation flow to create a new graph file and assign it to an existing GameObject in your project. 

![The Empty Graph Creation Flow window](images\vs-empty-graph-create-flow.png)

For more information on other ways to create a graph file, see [Create a new graph file](vs-create-graph.md).

## Create a Script Graph

To create a new Script Graph and assign it to an existing GameObject:

1. [!include[open-hierarchy-window](./snippets/vs-open-hierarchy-window.md)]

1. In the Hierarchy window, select the GameObject where you want to assign the new graph. 

2. [!include[visual-scripting-window](./snippets/vs-visual-scripting-window.md)]

3. Expand **Create new Script Graph**.

1. Select **on selected game object**. 

4. Choose a location to save the new graph file. 

1. Enter a name for the graph. 

1. Select **Save**.

    The new graph file automatically opens in a new window. 

![A new Script Graph, created with the empty graph creation flow with starter On Update and On Start Event nodes.](images\vs-new-graph-starter-nodes.png)

## Create a State Graph

To create a new State Graph and assign it to an existing GameObject:

1. [!include[open-hierarchy-window](./snippets/vs-open-hierarchy-window.md)]

1. In the Hierarchy window, select the GameObject where you want to assign the new graph. 

2. [!include[visual-scripting-window](./snippets/vs-visual-scripting-window.md)]

3. Expand **Create new State Graph**.

1. Select **on selected game object**. 

4. Choose a location to save the new graph file. 

1. Enter a name for the graph. 

1. Select **Save**.

    The new graph file automatically opens in a new window. 

## Next steps 

After you create a new graph, attach it to a Script Machine or State Machine to use it in your application. For more information, see [Attach a graph file to a Script Machine or State Machine](vs-attach-graph-machine.md).
