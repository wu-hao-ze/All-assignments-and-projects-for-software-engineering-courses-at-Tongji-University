# Use relations to debug

Relations are a useful tool to understand the dependencies between each port of a node. For example, to get the result of A + B with the Add node, you need to provide a value for A and B. Likewise, before invoking the Log node, you should provide a value for its Message input port.

![](images/vs-relations-debugging-show-relations.png)



Visual scripting uses this information in the background for Predictive Debugging. For example, if you tried to get the value of A + B without providing a value for A, the node would show up as orange to indicate that it fails in play mode.

![](images/vs-relations-debugging-predictive.png)


When that happens, you can use the warnings shown in the Graph Inspector to know what is missing.


Relations can also help understand the ports that are required and which ports are optional. For example, in the Get Child node (under fuzzy finder **Codebase** > **Unity Engine** > **Transform**), there is no need to connect the control ports if the goal is to get the transform value output.

Enable the Relations toggle in the toolbar for the inner connections of each node to be displayed.

> [!NOTE]
> You cannot edit relations. They are predefined for each type of node.
