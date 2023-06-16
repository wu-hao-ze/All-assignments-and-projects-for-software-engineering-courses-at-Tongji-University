# Add relations to a Custom C# node

> [!NOTE]
> To add logic to a node, you must create a Custom C# node and add ports. [!include[vs-tasks-note-end](./snippets/custom-c-nodes/vs-tasks-note-end.md)]

After you [add ports](vs-create-custom-node-add-ports.md) and [add logic](vs-create-custom-node-add-logic.md) to a node, relations help Visual Scripting correctly display a Custom C# node in a Script Graph. 

To add relations to a node: 

1. [!include[open-project-window](./snippets/vs-open-project-window.md)]

2. [!include[open-existing-external-code](./snippets/vs-open-existing-external-code.md)]

3. In your external editor, add relations in the format `$RelationType$($Port1$, $Port2$)`, where `$RelationType$` is the relation type you want to assign between the ports you specify as `$Port1$` or `$Port2$`. 

    For example, to assign relations to the previous example node: 

    ```C#
    using System;
    using Unity.VisualScripting;
    using UnityEngine;

    public class MyNode : Unit
    {
    [DoNotSerialize]
    public ControlInput inputTrigger;

    [DoNotSerialize]
    public ControlOutput outputTrigger;

    [DoNotSerialize]
    public ValueInput myValueA;

    [DoNotSerialize]
    public ValueInput myValueB;

    [DoNotSerialize]
    public ValueOutput result;

    private string resultValue;
    protected override void Definition()
    {
        inputTrigger = ControlInput("inputTrigger", (flow) =>
        {
            resultValue = flow.GetValue<string>(myValueA) + flow.GetValue<string>(myValueB) + "!!!";
            return outputTrigger;
        });
        outputTrigger = ControlOutput("outputTrigger");
        
        myValueA = ValueInput<string>("myValueA", "Hello ");
        myValueB = ValueInput<string>("myValueB", String.Empty);
        result = ValueOutput<string>("result", (flow) => resultValue);
        
        Requirement(myValueA, inputTrigger); //Specifies that we need the myValueA value to be set before the node can run.
        Requirement(myValueB, inputTrigger); //Specifies that we need the myValueB value to be set before the node can run.
        Succession(inputTrigger, outputTrigger); //Specifies that the input trigger port's input exits at the output trigger port. Not setting your succession also dims connected nodes, but the execution still completes.
        Assignment(inputTrigger,result);//Specifies that data is written to the result string output when the inputTrigger is triggered.
    }
    }

    ```

    For more information on relation types, see [Custom C# nodes](vs-create-custom-node.md#relation-types).

4. [!include[save-script](./snippets/vs-save-script.md)]

1. [!include[return-unity](./snippets/vs-return-unity.md)]

5. Do one of the following: 

    - [!include[open-graph-w-node](./snippets/custom-c-nodes/vs-open-graph-w-node.md)].
    - [!include[ff-add-node](./snippets/custom-c-nodes/vs-ff-add-node.md)]

6. In the [Graph toolbar](vs-interface-overview.md#the-graph-toolbar), enable **Relations**. 

    Visual Scripting displays the relations you assigned to the Custom C# node. If you used the previous code sample, the node's relations might look like the following image: 

    ![An image of the Graph window. The opened Script Graph has two groups that each contain a single Custom C# node: a yellow Before group and a purple After group. In the Before group, the Custom C# node, My Node, has no relations. Visual Scripting doesn't display any lines on the node to demonstrate a relation between ports. In the After group, the Custom C# node has two lines that branch from the ports of the My Value A port and the My Value B port. These two lines meet at the Input Trigger port. On the node, the line from the Input Trigger port separates into two lines. One line goes to the Output Trigger port, while the other goes to the Result output port.](images/vs-my-node-custom-node-relations.png)

## Next steps 

After you add relations to a node, you can choose to [add documentation](vs-create-custom-node-add-docs.md) or [customize the node with attributes](vs-create-custom-node-attributes-reference.md).
