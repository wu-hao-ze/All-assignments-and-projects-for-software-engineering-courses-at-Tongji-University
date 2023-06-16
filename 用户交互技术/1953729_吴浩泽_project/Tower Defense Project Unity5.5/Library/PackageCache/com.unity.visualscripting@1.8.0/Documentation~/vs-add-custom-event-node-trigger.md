# Add a Custom Event Trigger node

You can use a Custom Event Trigger node to trigger a matching Custom Event node in your application. For more information on custom Events, see [Custom Events](vs-custom-events.md).

> [!NOTE]
> Before you add a Custom Event Trigger node, you must add and configure a Custom Event node in a Script Graph. For more information, see [Add a Custom Event node](vs-add-custom-event-node.md).

To add a Custom Event Trigger node to a Script Graph: 

1. [Open the Script Graph](vs-open-graph-edit.md) where you want to add a Custom Event Trigger node. This can be the same graph or a different graph from where you added a Custom Event node.

2. [!include[open-fuzzy-finder](./snippets/vs-open-fuzzy-finder.md)]. 

1. Go to **Events**.

1. Select the **Custom Event Trigger** node to add it to the graph.

3. In the **Name** input port's field, enter the name of the Custom Event node you want to trigger, exactly as it appears on the Custom Event node, through one of the following methods: 

    - Enter the name in the field next to the **Name** input port.
    - Attach a node that outputs the name as a string value to the **Name** input port.

4. In the **GameObject** field, indicated by the GameObject icon on the node, specify the GameObject that you want to trigger your Event. This doesn't have to be the same GameObject as the Custom Event node. Do one of the following: 

    - Select the object picker (circle icon) and select the GameObject.
    - Attach a node to the field's data input port that outputs the GameObject. 
    - Leave the field as the default value of **This** to use the GameObject from your currently open Script Graph's Script Machine.

5. In the **Arguments** field, enter the number from your Custom Event node's **Arguments** field. 

    > [!NOTE]
    > All arguments on a Custom Event Trigger node must receive input values, even if the Custom Event node doesn't use those arguments. Otherwise, Visual Scripting displays an error in the Graph Inspector for the Custom Event Trigger node.

    ![An image of the Graph Editor and Graph Inspector. An On Keyboard Input node with its Key set to Space and its Action set to Hold connects to a Custom Event Trigger node. The node triggers the MyEvent Custom Event node, which connects to a Debug Log node to write the value of Arg. 0 to the console. The Custom Event Trigger node displays in orange because its Arg. 0 and Arg. 1 input ports don't receive any input values. The Graph Inspector displays two warnings that state that Arg. 0 is missing, and Arg. 1 is missing.](images/vs-events-custom-event-trigger-node-error.png)

## Next steps 

You can [add more nodes](vs-add-node-to-graph.md) and [connect them](vs-creating-connections.md) to create the trigger logic for your Custom Event node in the graph. 

You can also [create a Custom Scripting Event node](vs-create-own-custom-event-node.md).
