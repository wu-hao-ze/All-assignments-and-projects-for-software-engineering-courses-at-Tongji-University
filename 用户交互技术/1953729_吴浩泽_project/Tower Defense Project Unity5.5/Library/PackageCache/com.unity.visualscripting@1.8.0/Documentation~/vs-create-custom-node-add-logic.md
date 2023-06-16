# Add logic to a Custom C# node

> [!NOTE]
> To add logic to a node, you must create a Custom C# node and add ports. [!include[vs-tasks-note-end](./snippets/custom-c-nodes/vs-tasks-note-end.md)]

After you create a Custom C# node and [add ports](vs-create-custom-node-add-ports.md), you can add logic to a node. Add logic to tell Visual Scripting what the node does with any data it receives from its ports. 

To add logic to a node: 

1. [!include[open-project-window](./snippets/vs-open-project-window.md)]

2. [!include[open-existing-external-code](./snippets/vs-open-existing-external-code.md)]

3. In your external editor, add any logic for the node within the lambda expression that handles the assignment of the `inputTrigger`. For example, you can take the values of the [two `string` input ports added in the previous example](vs-create-custom-node-add-ports.md#add-value-ports) and concatenate them, as shown in the following code: 

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
        //The lambda to execute our node action when the inputTrigger port is triggered.
        inputTrigger = ControlInput("inputTrigger", (flow) =>
        {
            //Making the resultValue equal to the input value from myValueA concatenating it with myValueB.
            resultValue = flow.GetValue<string>(myValueA) + flow.GetValue<string>(myValueB) + "!!!";
            return outputTrigger;
        });
        outputTrigger = ControlOutput("outputTrigger");
        
        myValueA = ValueInput<string>("myValueA", "Hello ");
        myValueB = ValueInput<string>("myValueB", String.Empty);
        result = ValueOutput<string>("result", (flow) => resultValue);
    }
    }
    ```

4. [!include[save-script](./snippets/vs-save-script.md)]

1. [!include[return-unity](./snippets/vs-return-unity.md)]

5. Do one of the following: 

    - [!include[open-graph-w-node](./snippets/custom-c-nodes/vs-open-graph-w-node.md)].
    - [!include[ff-add-node](./snippets/custom-c-nodes/vs-ff-add-node.md)]

## Next steps 

After you add logic to a node, [add relations](vs-create-custom-node-add-relations.md) to ensure that the node displays correctly in Visual Scripting. 
