# Attach a graph file to a Script Machine or State Machine

To use a Script Graph or State Graph file in your project, you must attach it to a Script Machine or State Machine. 

A Script Machine or State Machine is a component. Components attach to GameObjects, and help define their behavior. For more information on components and GameObjects, see [Using components](https://docs.unity3d.com/2021.1/Documentation/Manual/UsingComponents.html) or [GameObjects](https://docs.unity3d.com/2021.1/Documentation/Manual/GameObjects.html) in the Unity User Manual.

## Add a Script Machine or State Machine component to a GameObject

1. [!include[open-hierarchy](./snippets/vs-open-hierarchy-window.md)]

1. In the Hierarchy window, select a GameObject where you'd like to add a Script Machine or State Machine. 

2. [!include[open-inspector-window](./snippets/vs-open-inspector-window.md)]  

3. In the GameObject's Inspector window, select **Add Component**. 
    
    The Components menu opens.

4. Do one of the following: 
    - To add a Script Machine or State Machine, in the Components menu, go to **Visual Scripting** and select **Script Machine** or **State Machine**. 
    - Use the search bar to find the Script Machine or State Machine component. 

The new Script Machine or State Machine component appears in the Inspector window for the GameObject:

![A new blank Script Machine component in the Inspector window for a GameObject](images\vs-script-machine-blank.png)

## Attach a graph file to the Script Machine or State Machine 

1. In the Inspector window, locate your Script Machine or State Machine component. 

1. Set the **Source** to **Graph**. 

2. Do one of the following: 
    - In the **Graph** field, select the object picker (circle icon) and choose a compatible graph file from your project. 
    - Click and drag a file from your Project window and drop it into the **Graph** field.

For more information on how to create Script or State Graphs, see [Create a new graph file](vs-create-graph.md).

## Next steps 

After you attach a graph to a Script Machine or State Machine, you can open the graph and edit. For more information, see [Open a graph file](vs-open-graph-edit.md).
