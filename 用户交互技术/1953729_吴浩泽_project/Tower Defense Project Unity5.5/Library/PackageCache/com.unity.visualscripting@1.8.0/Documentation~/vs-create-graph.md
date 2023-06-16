# Create a new graph file

To create a new Visual Scripting graph file, you can use the Unity Editor's Project window, Visual Scripting's empty graph creation flow, or create a graph from a Script Machine or State Machine component. 

## Use the Project window

If you [create a graph with the Project window](vs-create-graph-project-window.md), the graph is blank. It contains no starter nodes, and isn't attached to any existing components in your project. 

For more information on the Unity Editor's Project window, see [The Project window](https://docs.unity3d.com/Manual/ProjectView.html) in the Unity User Manual.

## Use the empty graph creation flow

If you use the empty graph creation flow, you have a few options for how to create your graph: 

- You can choose to [create an unassigned graph](vs-create-graph-unassigned-flow.md). The graph isn't assigned to a GameObject. 
- You can choose to [create a graph and assign it to an existing GameObject](vs-create-graph-assign-existing-gameobject.md). Visual Scripting creates the required components on the GameObject for you to use your graph in your project.
- You can choose to [create a graph and assign it to a new GameObject](vs-create-graph-assign-new-gameobject.md). Visual Scripting creates a new GameObject with the required components for you to use your graph in your project.

 For more information about GameObjects, see [GameObjects](https://docs.unity3d.com/Manual/GameObjects.html) in the User Manual.
 
 Any graph you create with the empty graph creation flow contains one or two initial nodes to help you get started with your graph.  


> [!TIP]
> To keep your project organized, place your Visual Scripting graphs in a **Graphs** folder inside your project's **Assets** folder.

## Use a Script Machine or State Machine

You can also create a blank graph file directly on the Script Machine or State Machine component where you want to use it. For more information, see [Create a graph on a Script Machine or State Machine](vs-create-graph-on-machine.md). 

